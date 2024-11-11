using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(Program));
    });

builder.Services.AddDbContext<HotelContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));

// Agregar IWebHostEnvironment al contenedor de servicios
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

// Configurar la autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuarios/Login";
        options.LogoutPath = "/Usuarios/Logout";
    });

// Configurar la localización
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("es-ES"),
        new CultureInfo("en-US")
    };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("es-ES");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Agregar UseAuthentication antes de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();