namespace SharedLibrarySolution.Responses
{
    public class PageResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public IEnumerable<T> Items { get; set; } = new List<T>(); // dùng IEnumerable vì chỉ đọc kh cập nhật, hữu ích khi làm việc với LINQ

        public PageResponse() { }

        public PageResponse(IEnumerable<T> items, int totalItems, int pageNumber, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
