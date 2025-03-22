using CRUD_NoSQL.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CRUD_NoSQL.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMongoCollection<Usuario> _usuariosCollection;

        public AuthController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Crud_NoSQL");
            _usuariosCollection = database.GetCollection<Usuario>("Usuarios");
        }

        // Vista de registro
        public IActionResult Register()
        {
            return View();
        }

        // Registro de usuario (admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Nombre", "Correo", "Contraseña")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el usuario ya existe
                    var existingUser = await _usuariosCollection.Find(u => u.Correo == usuario.Correo).FirstOrDefaultAsync();
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("", "El correo ya está registrado.");
                        return View(usuario);
                    }

                    // Insertar el nuevo usuario
                    await _usuariosCollection.InsertOneAsync(usuario);
                    return RedirectToAction(nameof(Login));
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("", "Error registrando el usuario: " + ex.Message);
                }
            }

            return View(usuario);
        }

        // Vista de login
        public IActionResult Login()
        {
            return View();
        }

        // Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Correo", "Contraseña")] Usuario usuario)
        {
            var foundUser = await _usuariosCollection.Find(u => u.Correo == usuario.Correo && u.Contraseña == usuario.Contraseña).FirstOrDefaultAsync();

            if (foundUser != null)
            {
                // Guardar en sesión que el usuario está autenticado
                HttpContext.Session.SetString("UserId", foundUser.Id);
                return RedirectToAction("Index", "Productos");
            }

            ModelState.AddModelError("", "Correo o contraseña incorrectos");
            return View(usuario);
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }
    }
}
