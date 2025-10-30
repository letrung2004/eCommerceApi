using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductService.Presentation.Entities
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
