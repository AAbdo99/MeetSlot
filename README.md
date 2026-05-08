# MeetSlot

MeetSlot er et bookingsystem for moterom bygget med ASP.NET Core 8.
Prosjektet tilbyr JWT-autentisering, rolle- og policybasert tilgangskontroll, bookinglogikk med konfliktkontroll, global feilhondtering og request-logging.

## Funksjonalitet

- Registrering og innlogging med JWT
- Booking-flyt: opprett, hent, slett og ledige tider
- Policybasert tilgangskontroll for booking-eierskap/admin
- Standardisert feilsvar med `traceId`
- Swagger for API-testing
- Razor Pages-visning av rom pa `/rooms`

## Teknologi

- ASP.NET Core 8 Web API
- Entity Framework Core 8 + Npgsql
- PostgreSQL 16
- JWT Bearer auth
- BCrypt passordhashing
- Docker + Docker Compose

## Prosjektstruktur

- `Controllers/` HTTP-endepunkter
- `Services/` forretningslogikk
- `Repositories/` databaseadgang
- `Data/` `MeetSlotDbContext`
- `Models/` domenemodeller
- `Dtos/` input/output-modeller
- `Middleware/` logging og global exception handling
- `Authorization/` custom krav og handlers
- `Validation/` valideringsattributter
- `Pages/` Razor Pages

## API-endepunkter

### Auth

| Metode | Endepunkt | Beskrivelse |
|---|---|---|
| `POST` | `/api/Auth/register` | Opprett bruker |
| `POST` | `/api/Auth/login` | Logg inn og hent JWT-token |

### Booking

Alle booking-endepunkter krever `Authorization: Bearer <token>`.

| Metode | Endepunkt | Beskrivelse |
|---|---|---|
| `POST` | `/api/Booking` | Opprett booking |
| `GET` | `/api/Booking` | Hent bookinger (admin: alle, bruker: egne) |
| `GET` | `/api/Booking/mine` | Hent kun mine bookinger |
| `GET` | `/api/Booking/{id}` | Hent booking per id |
| `DELETE` | `/api/Booking/{id}` | Slett booking |
| `GET` | `/api/Booking/available-slots?meetingRoomId={id}&date={yyyy-MM-dd}` | Hent ledige tider |

## Sikkerhet

- Bruker-id hentes fra JWT `NameIdentifier` claim, aldri fra request-body.
- `BookingOwnerOrAdmin` policy beskytter booking som tilhorer andre.
- `KunAdmin` policy brukes der kun admin skal ha tilgang.

## Logging og feilhondtering

- `RequestLoggingMiddleware` logger request/response.
- `GlobalExceptionMiddleware` returnerer konsistent feilsvar:

```json
{
  "error": "...",
  "status": 400,
  "traceId": "..."
}
```

- Modellvalidering returneres med samlet `details`-struktur.

## Krav

- .NET SDK 8.x
- Docker Desktop
- (Valgfritt) `dotnet-ef` CLI

Installer `dotnet-ef` ved behov:

```bash
dotnet tool install --global dotnet-ef
```

## Kjoring

### A) Docker Compose (anbefalt)

Bygger og starter API + database:

```bash
docker compose up --build
```

- API: `http://localhost:5182`
- Swagger: `http://localhost:5182/swagger`

Merk: API-containeren bruker intern DB-host `db` pa port `5432`.

### B) Kjor API lokalt mot Docker-database

Start kun DB i Docker:

```bash
docker compose up -d db
```

Sett connection string lokalt med riktig host-port `5433`:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=meetslot;Username=postgres;Password=postgres"
```

Sett JWT-verdier lokalt:

```bash
dotnet user-secrets set "Jwt:Issuer" "MeetSlot"
dotnet user-secrets set "Jwt:Audience" "MeetSlotUsers"
dotnet user-secrets set "Jwt:Key" "ThisIsADevelopmentSecretKeyForMeetSlot12345"
```

Kjor API:

```bash
dotnet run
```

## Database og seed-data

Ved oppstart kjores migreringer automatisk i `Program.cs` (`db.Database.Migrate()`).

Seed-data inkluderer blant annet:

- Brukere:
  - `admin@meetslot.local` (Admin)
  - `user@meetslot.local` (User)
- Rom:
  - `Nordic Room`
  - `Fjord Room`
  - `Aurora Room`

## Feilsoking

- Feil mot DB pa `5432` ved lokal kjoring: bruk `Port=5433` i User Secrets.
- User Secrets overstyrer `appsettings.*` i Development.
- Ugyldig/manglende JWT gir `401`, manglende tilgang gir `403` eller `404` avhengig av policy/ressurs.

## Team

- Semir: Auth/Security
- Ashwaq: DB/EF Core
- Ikram: Bookinglogikk
- Marion: Testing/Docs/DevOps
