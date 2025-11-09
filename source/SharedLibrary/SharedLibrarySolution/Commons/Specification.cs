using Serilog;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;

namespace SharedLibrarySolution.Commons
{
    //Specification Pattern giúp đóng gói logic lọc này vào một class riêng biệt
    //có thể tái sử dụng, test độc lập, và dễ mở rộng.
    public abstract class Specification<T>
    {
        /// <summary>
        /// Expression<Func<T, bool>>: Đây là một biểu thức lambda kiểu Func<T, bool>, được dùng để lọc dữ liệu (T là kiểu của đối tượng cần kiểm tra).
        /// </summary>
        public Expression<Func<T, bool>> Criteria { get; protected set; } = _ => true;

        public Specification() { }
        /// <summary>
        /// Cho phép khởi tạo Specification với một tiêu chí lọc cụ thể.
        /// </summary>
        /// <param name="criteria"></param>
        public Specification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
    }
}
