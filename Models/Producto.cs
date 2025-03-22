using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CRUD_NoSQL.Models
{
    public class Producto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("Nombre")]
        public string Nombre { get; set; } = null!;

        [BsonElement("Descripcion")]
        public string Descripcion { get; set; } = null!;

        [BsonElement("Ingredientes")]
        public string Ingredientes { get; set; } = null!;

        [BsonElement("TipoPlatillo")]
        public string TipoPlatillo { get; set; } = null!;

        [BsonElement("Precio")]
        public decimal Precio { get; set; }

        [BsonElement("ImagenUrl")]
        public string ImagenUrl { get; set; } = null!;
    }
}
