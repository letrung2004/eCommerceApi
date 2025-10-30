namespace ApiGateway.Presentation.Middlewares
{
    //theo dõi (trace) toàn bộ request xuyên suốt các service.
    public sealed class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        public const string HeaderName = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext ctx)
        {
            var cid = ctx.Request.Headers[HeaderName].FirstOrDefault()
                      ?? Guid.NewGuid().ToString("N");

            ctx.Items[HeaderName] = cid;
            ctx.Response.Headers[HeaderName] = cid;
            await _next(ctx);
        }
    }
}
