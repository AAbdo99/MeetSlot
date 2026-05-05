namespace MeetSlot.Exceptions
{
    // Samler alle exception-meldinger i ett sted.
    // Fyll inn tekstene under med ønsket, profesjonell formulering.
    public static class ExceptionMessages
    {
        public static class Autentisering
        {
            // Brukes av: ConflictException i AuthService.RegisterAsync
            public const string EpostAlleredeRegistrert = "E-postadressen er allerede registrert";

            // Brukes av: UnauthorizedException i AuthService.LoginAsync
            public const string UgyldigeInnloggingsdata = "Ugyldig e-postadresse eller passord";
        }

        public static class Booking
        {
            // Brukes av: NotFoundException i BookingService.CreateBookingAsync/GetAvailableSlotsAsync
            public const string MoteromIkkeFunnet = "Møterommet ble ikke funnet";

            // Brukes av: NotFoundException i BookingService.CreateBookingAsync
            public const string BrukerIkkeFunnet = "Brukeren ble ikke funnet";

            // Brukes av: ConflictException i BookingService.CreateBookingAsync
            public const string Tidskonflikt = "Tidsrommet er allerede opptatt";

            // Brukes av: NotFoundException i BookingService.GetBookingByIdAsync/DeleteBookingAsync
            public const string BookingIkkeFunnet = "Bookingen ble ikke funnet";
        }

        public static class Felles
        {
            // Brukes av: GlobalExceptionMiddleware ved ukjente feil (500)
            public const string UventetFeil = "En uventet feil oppstod. Vennligst prøv igjen senere.";
        }
    }
}
