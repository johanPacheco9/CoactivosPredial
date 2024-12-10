using Radzen;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Coactivos_Predial.Shared.Services.SCoactivos;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using Coactivos_Predial.Auth;
using Coactivos_Predial.Shared.Services.SLogin;
using Coactivos_Predial.Shared.Services.SPropietarios;
using Coactivos_Predial.Shared.Services.Spersuasivo;
using Coactivos_Predial.Shared.Services;
using Coactivos_Predial.Shared.Components.ComponentServices.BusquedaService;
using Coactivos_Predial.Shared.Services.SParametros;
using Coactivos_Predial.Shared.Services.SImportados;
using Coactivos_Predial.Shared.Services.FileHandler;
using Coactivos_Predial.Shared.Services.SUser;
using Coactivos_Predialp.Shared.Services.SPropietarios;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Coactivos_Predial.Shared.Services.SDatabase;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<ICoactivosService, CoactivosService>();
builder.Services.AddScoped<IArchivoService, ArchivoService>();
builder.Services.AddDbContext<AppDbContext>(ServiceLifetime.Scoped);
builder.Services.AddScoped<IPersuasivoService, PersuasivoService>();
builder.Services.AddScoped<ICoactivosService, CoactivosService>();
builder.Services.AddScoped<IPrediosService, PrediosService>();
builder.Services.AddScoped<IParametroService, ParametroService>();
builder.Services.AddScoped<IimportadosService, ImportadosService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<IpropietariosService, PropietariosService>();
builder.Services.AddServerSideBlazor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.Zero;
        options.AccessDeniedPath = "/Forbidden/";

    });
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddSingleton<BusquedaService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuthorization();

builder.Services.AddDataProtection()
    .SetApplicationName("myapp");


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapBlazorHub();
app.MapControllers();
app.MapFallbackToPage("/_Host");

var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};

app.UseCookiePolicy(cookiePolicyOptions);


app.Run();
