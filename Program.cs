using CRUD_NoSQL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddControllersWithViews();

// Configuración de la conexión a MongoDB
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var client = new MongoClient("mongodb://localhost:27017");
    return client;
});

// Configuración de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Duración de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configuración para usar cookies de autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
    });

// Agregar la base de datos MongoDB
builder.Services.AddSingleton<IMongoCollection<Usuario>>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase("Crud_NoSQL");
    return database.GetCollection<Usuario>("Usuarios");
});

var app = builder.Build();

// Configurar el pipeline de la aplicación.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Habilitar HTTPS y archivos estáticos
app.UseHttpsRedirection();
app.UseStaticFiles();

// ⚠ CORRECCIÓN: Habilitar la sesión antes que la autenticación
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Configurar las rutas de la aplicación
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// Iniciar la aplicación
app.Run();
