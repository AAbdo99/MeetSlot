using System.Text.Json;
using MeetSlot.Exceptions;

namespace MeetSlot.Middleware
{
    // Fanger exceptions globalt og returnerer et konsistent JSON-feilformat.
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                // SIKKERHET/ROBUSTHET: Hvis response allerede har startet, re-throw eksepsjonen.
                // Det betyr at en downstream handler har allerede skrevet headers/body til klienten.
                // Vi kan ikke sette StatusCode eller skrive egen JSON-respons, så vi lar exceptionem propagere.
                if (context.Response.HasStarted)
                {
                    throw;
                }

                _logger.LogWarning(ex,
                    "App-feil ({StatusCode}) på {Method} {Path}: {Message}",
                    ex.StatusCode,
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message);

                // Kjente app-feil gir kontrollert statuskode og melding.
                await WriteErrorResponseAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                // SIKKERHET/ROBUSTHET: Hvis response allerede har startet, re-throw eksepsjonen.
                if (context.Response.HasStarted)
                {
                    throw;
                }

                // Ukjente feil logges og returnerer 500 uten intern detalj.
                _logger.LogError(ex, "Uventet feil under behandling av request {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                

                await WriteErrorResponseAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    ExceptionMessages.Felles.UventetFeil);
            }
        }

        private static async Task WriteErrorResponseAsync(
            HttpContext context,
            int statusCode,
            string message)
        {
            // Response er garantert IKKE startet her – HasStarted-sjekken skjer i InvokeAsync før denne kalles.
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            // Felles responsmodell for alle feil fra middleware.
            var payload = new
            {
                error = message,
                status = statusCode,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
