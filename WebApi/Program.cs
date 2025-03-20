using AutoMapper;
using BusinessLogic.Data;
using BusinessLogic.Logic;
using Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using WebApi.Dtos;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configurar servicios (similar a lo que harías en Startup.ConfigureServices)
builder.Services.AddDbContext<MarketDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Registrar repositorios y otros servicios
builder.Services.AddTransient<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddControllers();

// Configuración de CORS
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsRule", rule =>
    {
        rule.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("https://tudominio.com", "https://otrodominio.com"); // Ajusta los dominios según tus necesidades
    });
});

var app = builder.Build();

// Ejecutar migraciones automáticamente al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

    try
    {
        var context = services.GetRequiredService<MarketDbContext>();
        await context.Database.MigrateAsync();
        await MarketDbContextData.CargarDataAsync(context, loggerFactory);
    }
    catch (Exception e)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(e, "Errores en el proceso de migración");
    }
}

// Configuración del pipeline de la aplicación (similar a Startup.Configure)
/*if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}*/

app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Activar CORS
app.UseCors("CorsRule");

app.UseAuthorization();

// Configurar rutas de los controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
