namespace MeetSlot.Middleware
{
    // Logger alle innkommende HTTP-requests i ett sentralt punkt.
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Viser metode + path for enklere feilsoking i logs.
            _logger.LogInformation(
                "HTTP {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await _next(context);
        }
    }
}
