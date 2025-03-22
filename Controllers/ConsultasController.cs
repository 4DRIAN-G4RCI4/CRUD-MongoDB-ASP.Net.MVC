using CRUD_NoSQL.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD_NoSQL.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly IMongoCollection<Producto> _productosCollection;

        public ConsultasController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Crud_NoSQL");
            _productosCollection = database.GetCollection<Producto>("Productos");
        }

        // Consulta 1: Productos más caros
        public async Task<IActionResult> ProductosMasCaros()
        {
            var productosCaros = await _productosCollection
                .Find(_ => true)  //&// aqui hacemos el findo 
                .SortByDescending(p => p.Precio) // Ordenar por precio de mayor a menor
                .Limit(5) // Limitar a los 5 productos más caros  si queremos poner mas modificamos el limite 
                .ToListAsync(); 

            return View(productosCaros);
        }



        // Consulta 2: Productos más baratos
        public async Task<IActionResult> ProductosMasBaratos()
        {
            var productosBaratos = await _productosCollection
                .Find(_ => true)
                .SortBy(p => p.Precio) // Ordenar por precio de menor a mayor
                .Limit(5) // Limitar a los 5 productos más baratos
                .ToListAsync();

            return View(productosBaratos);
        }

        // Consulta 3: Promedio de precios de los productos
        public async Task<IActionResult> PromedioPrecios()
        {
            var promedioPrecio = await _productosCollection
                .Aggregate()
                .Group(new BsonDocument {
            { "_id", BsonNull.Value }, // El _id no se usa para este caso
            { "promedio", new BsonDocument("$avg", "$Precio") } // Calculamos el promedio del campo "Precio"
                })
                .FirstOrDefaultAsync(); // Obtener solo el primer resultado (en este caso, uno solo)

            return View(promedioPrecio);
        }

        // Consulta 4: Productos por tipo de platillo
        public async Task<IActionResult> ProductosPorTipo(string tipo)
        {
            var productosPorTipo = await _productosCollection
                .Find(p => p.TipoPlatillo == tipo)
                .ToListAsync();

            return View(productosPorTipo);
        }

        // Consulta 5: Cantidad total de productos por tipo de platillo
        public async Task<IActionResult> CantidadProductosPorTipo()
        {
            var cantidadPorTipo = await _productosCollection
                .Aggregate()
                .Group(new BsonDocument {
            { "_id", "$TipoPlatillo" }, // Agrupar por el campo "TipoPlatillo"
            { "cantidad", new BsonDocument("$sum", 1) } // Contamos cuántos productos hay por cada tipo
                })
                .ToListAsync(); // Obtener todos los resultados

            return View(cantidadPorTipo);
        }

    }
}
