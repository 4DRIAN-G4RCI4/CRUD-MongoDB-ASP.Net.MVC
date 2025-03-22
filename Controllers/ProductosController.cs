using CRUD_NoSQL.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CRUD_NoSQL.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IMongoCollection<Producto> _productosCollection;

        public ProductosController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Crud_NoSQL");
            _productosCollection = database.GetCollection<Producto>("Productos");
        }

        public IActionResult Index()
        {
            // Verificar si el usuario está autenticado
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth"); // Redirigir al login si no está autenticado
            }

            var productos = _productosCollection.Find(_ => true).ToList();
            return View(productos);
        }

        // Acción para mostrar el formulario de creación (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Acción para guardar el nuevo producto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre", "Descripcion", "Ingredientes", "TipoPlatillo", "Precio", "ImagenUrl")] Producto producto)
        {
            producto.Id = ObjectId.GenerateNewId().ToString(); // Generar un nuevo ID

            try
            {
                // Insertar el nuevo producto en la colección
                await _productosCollection.InsertOneAsync(producto);
                return RedirectToAction(nameof(Index)); // Redirigir a la lista de productos
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error insertando el documento: {ex.Message}");
                ModelState.AddModelError("", "Error insertando el documento");
                return View(producto); // Si hubo error, regresar a la vista de creación
            }
        }

        // Acción para mostrar el formulario de edición (GET)
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            // Buscar el producto por ID
            var producto = await _productosCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (producto == null)
                return NotFound();

            return View(producto); // Mostrar el formulario con los datos del producto
        }

        // Acción para guardar los cambios del producto editado (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id", "Nombre", "Descripcion", "Ingredientes", "TipoPlatillo", "Precio", "ImagenUrl")] Producto producto)
        {
            if (id != producto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Reemplazar el producto existente por el producto actualizado
                await _productosCollection.ReplaceOneAsync(p => p.Id == id, producto);
                return RedirectToAction(nameof(Index)); // Redirigir a la lista de productos
            }

            return View(producto); // Si no es válido, mostrar el formulario con los datos del producto
        }

        // Acción para mostrar el formulario de eliminación (GET)
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            // Buscar el producto por ID
            var producto = await _productosCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (producto == null)
                return NotFound();

            return View(producto); // Mostrar el producto a eliminar
        }

        // Acción para eliminar el producto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, [Bind("Id", "Nombre", "Precio")] Producto producto)
        {
            await _productosCollection.DeleteOneAsync(p => p.Id == id); // Eliminar el producto de la base de datos
            return RedirectToAction(nameof(Index)); // Redirigir a la lista de productos
        }
    }
}
