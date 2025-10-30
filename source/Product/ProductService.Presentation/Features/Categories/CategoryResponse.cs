using ProductService.Presentation.Entities;
using SharedLibrarySolution.Interfaces;

namespace ProductService.Presentation.Features.Categories
{
    public class CategoryResponse : IMapFrom<Category>
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ParentId { get; set; } = string.Empty; //danh mục cha-con - hỗ trợ danh mục lồng nhau
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
