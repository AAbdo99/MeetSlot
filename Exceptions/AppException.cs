namespace MeetSlot.Exceptions
{
    // Felles base-exception for appen, med HTTP-statuskode som middleware kan bruke i respons.
    public abstract class AppException : Exception
    {
        protected AppException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        // Statuskode som sendes tilbake i API-responsen.
        public int StatusCode { get; }
    }
}
