namespace MeetSlot.Exceptions
{
    // Brukes når requesten er gyldig, men bryter en regel (f.eks. dobbeltbooking).
    public class ConflictException : AppException
    {
        public ConflictException(string message)
            : base(message, StatusCodes.Status409Conflict)
        {
        }
    }
}
