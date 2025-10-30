using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductService.Presentation.Entities
{
    public class Promotion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AppliesToType { get; set; } = string.Empty; // "Product" hoặc "Category"

        [BsonRepresentation(BsonType.ObjectId)]
        public string TargetId { get; set; } = string.Empty; // ID của sản phẩm hoặc danh mục
        public decimal Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
