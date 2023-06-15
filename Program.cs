using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos;
using ProyectoIdentity.Services;

var builder = WebApplication.CreateBuilder(args);

//Cadena de Conexión
builder.Services.AddDbContext<ApplicationDbContext>(
    opciones => opciones.UseSqlServer(builder.Configuration
    .GetConnectionString("ConexionSql")));

//Agrega el Servicio de Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

//Línea Url de retorno al Acceder
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Cuentas/Acceso");
    options.AccessDeniedPath = new PathString("/Cuentas/Bloqueado");
});

//Opciones de Configuración del Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
});

//Agregar autenticación de facebook
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "230137856465165";
    options.AppSecret = "57117970f5f69c172bf7fc12231cc7e5";
});

//Agregar autenticación de Google
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "438354083296-t3uk421l08lq304rqv6ejrsg7relac2p.apps.googleusercontent.com";
    options.ClientSecret = "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";
});

//Se agrega IEmailSender
builder.Services.AddTransient<IEmailSender, MailJetEmailSender>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//Agrega la Autenticación
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();