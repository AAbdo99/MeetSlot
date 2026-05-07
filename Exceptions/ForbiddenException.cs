namespace MeetSlot.Exceptions
{
    // Brukes nÃ¥r en innlogget bruker mangler rettigheter til en handling.
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message)
            : base(message, StatusCodes.Status403Forbidden)
        {
        }
    }
}
