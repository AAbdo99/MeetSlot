namespace MeetSlot.Exceptions
{
    // Brukes når innlogging/token er ugyldig for handlingen som ble forsøkt.
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message)
            : base(message, StatusCodes.Status401Unauthorized)
        {
        }
    }
}
