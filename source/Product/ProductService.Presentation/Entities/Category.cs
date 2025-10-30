using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductService.Presentation.Entities
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ParentId { get; set; } = string.Empty; //danh mục cha-con - hỗ trợ danh mục lồng nhau
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
