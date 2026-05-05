namespace MeetSlot.Exceptions
{
    // Brukes når en ressurs ikke finnes i databasen.
    public class NotFoundException : AppException
    {
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound)
        {
        }
    }
}
