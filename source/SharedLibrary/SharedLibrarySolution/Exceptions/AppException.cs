namespace SharedLibrarySolution.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; } = 400;

        public AppException(string message)
            : base(message)
        {
        }
    }
}
