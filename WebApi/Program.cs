// NetMarket/Program.cs
using BusinessLogic.Data;
using BusinessLogic.Logic;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using WebApi.Dtos;
using WebApi.Middleware;
using Microsoft.AspNetCore.Authorization; // Aseg�rate de que este import est� presente
using Microsoft.AspNetCore.Mvc.Authorization; // Aseg�rate de que este import est� presente

var builder = WebApplication.CreateBuilder(args);

// 1) Add DbContexts
builder.Services.AddDbContext<MarketDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<SeguridadDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("IdentitySeguridad")));

// 2) Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(
        ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true)));

// 3) Identity con Roles
builder.Services
    .AddIdentity<Usuario, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<SeguridadDbContext>()
    .AddDefaultTokenProviders();

// 3.1) Deshabilitar redirecciones de login para APIs
builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    opt.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

// 4) JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"])),
            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = false
        };

        // A�ADIDO: Eventos para depuraci�n del JWT Bearer
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (!string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"[DEBUG JWT Event] OnMessageReceived: Token encontrado en cabecera. Longitud: {token.Length}");
                }
                else
                {
                    Console.WriteLine("[DEBUG JWT Event] OnMessageReceived: No se encontr� token en la cabecera Authorization.");
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[DEBUG JWT Event] OnAuthenticationFailed: Error de autenticaci�n: {context.Exception.Message}");
                if (context.Exception.InnerException != null)
                {
                    Console.WriteLine($"[DEBUG JWT Event] Inner Exception: {context.Exception.InnerException.Message}");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("[DEBUG JWT Event] OnTokenValidated: Token validado exitosamente. Usuario autenticado.");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();


// 5) Otros servicios (CORS, AutoMapper, Repositorios�)
builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy("CorsRule", p =>
    p.AllowAnyHeader()
     .AllowAnyMethod()
     .WithOrigins("http://localhost:5173")
     .AllowCredentials()));
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericSeguridadRepository<>), typeof(GenericSeguridadRepository<>));
builder.Services.AddTransient<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICarritoCompraRepository, CarritoCompraRepository>();
builder.Services.AddScoped<IOrdenCompraService, OrdenCompraService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 6) (Opcional) Registrar expl�citamente TimeProvider
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

// A�ADE ESTAS L�NEAS PARA DEPURACI�N (ya las tienes)
Console.WriteLine($"[DEBUG Program.cs] Key for validation: '{app.Configuration["Token:Key"]}'");
Console.WriteLine($"[DEBUG Program.cs] Issuer for validation: '{app.Configuration["Token:Issuer"]}'");
Console.WriteLine($"[DEBUG Program.cs] Audience for validation: '{app.Configuration["Token:Audience"]}'");

// 7) Migraciones + Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logFactory = services.GetRequiredService<ILoggerFactory>();

    try
    {
        // Market
        var marketCtx = services.GetRequiredService<MarketDbContext>();
        await marketCtx.Database.MigrateAsync();
        await MarketDbContextData.CargarDataAsync(marketCtx, logFactory);

        // Seguridad
        var segCtx = services.GetRequiredService<SeguridadDbContext>();
        await segCtx.Database.MigrateAsync();

        // Seed Roles
        var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = new[] { "Admin", "Cliente" };
        foreach (var roleName in roles)
        {
            if (!await roleMgr.RoleExistsAsync(roleName))
                await roleMgr.CreateAsync(new IdentityRole(roleName));
        }

        // Seed Usuario Admin
        var userMgr = services.GetRequiredService<UserManager<Usuario>>();
        var adminEmail = "admin@correo.com";
        if (await userMgr.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new Usuario
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                Nombre = "Admin",
                Apellido = "User",
                Imagen = ""
            };
            var res = await userMgr.CreateAsync(admin, "Admin123*");
            if (res.Succeeded)
                await userMgr.AddToRoleAsync(admin, "Admin");
        }
    }
    catch (Exception ex)
    {
        var logger = logFactory.CreateLogger("Program");
        logger.LogError(ex, "Error durante migraciones o seeding");
    }
}

// 8) Pipeline
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors", "?code={0}");

// ORDEN CR�TICO:
app.UseRouting();
app.UseCors("CorsRule");
app.UseAuthentication(); // <-- DESCOMENTADO
app.UseAuthorization();  // <-- DESCOMENTADO
app.MapControllers();
app.Run();
