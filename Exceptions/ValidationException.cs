namespace MeetSlot.Exceptions
{
    // Brukes for valideringsfeil som oppstår i service-laget.
    public class ValidationException : AppException
    {
        public ValidationException(string message)
            : base(message, StatusCodes.Status400BadRequest)
        {
        }
    }
}
