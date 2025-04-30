using AutoMapper;
using BusinessLogic.Data;
using BusinessLogic.Logic;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Dtos;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Conexiones a bases de datos
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var identidadConnection = builder.Configuration.GetConnectionString("IdentitySeguridad");

// DbContexts
builder.Services.AddDbContext<MarketDbContext>(opt =>
    opt.UseSqlServer(defaultConnection));
builder.Services.AddDbContext<SeguridadDbContext>(opt =>
    opt.UseSqlServer(identidadConnection));

// Identity
builder.Services.AddIdentity<Usuario, IdentityRole>(options => {
    // Opciones de contraseña (ajusta según tu política)
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<SeguridadDbContext>()
.AddSignInManager<SignInManager<Usuario>>();

// Token service
builder.Services.AddScoped<ITokenService, TokenService>();

// Authentication con JWT
var tokenSettings = builder.Configuration.GetSection("Token");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(tokenSettings["Key"])
            ),
            ValidateIssuer = true,
            ValidIssuer = tokenSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = tokenSettings["Audience"],
            ClockSkew = TimeSpan.Zero
        };
        opt.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine($"JWT Error: {ctx.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });

// Autorización
builder.Services.AddAuthorization();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Repositorios y otros servicios
builder.Services.AddTransient<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsRule", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins("http://localhost:5173", "https://otrodominio.com");
    });
});

var app = builder.Build();

// Migraciones y seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

    try
    {
        var marketContext = services.GetRequiredService<MarketDbContext>();
        await marketContext.Database.MigrateAsync();
        await MarketDbContextData.CargarDataAsync(marketContext, loggerFactory);

        var identityContext = services.GetRequiredService<SeguridadDbContext>();
        await identityContext.Database.MigrateAsync();

        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        await SeguridadDbContextData.SeedUserAsync(userManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "Error al aplicar migraciones o seed");
    }
}

// Pipeline
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("CorsRule");

app.UseAuthentication();
app.UseAuthorization();

// Mapear controllers con attribute routing
app.MapControllers();

// Opcional: rutas convencionales
// app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
