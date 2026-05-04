# MeetSlot

Denne README-filen er laget som en midlertidig teamreferanse for prosjektet. Hensikten er a samle prosjektbeskrivelse, teknologivalg, rollefordeling, status og neste steg pa ett sted. Den kan oppdateres fortlopende eller erstattes senere med en mer formell versjon.

## Prosjektbeskrivelse
MeetSlot er et bookingsystem for moterom. Malet er a utvikle en enkel, palitelig og robust applikasjon der brukere kan:
- se tilgjengelige tider
- booke moterom
- avbooke moterom

Systemet skal ha rollebasert tilgang med to hovedroller:
- Admin
- Bruker

Brukere skal i hovedsak kunne handtere egne bookinger, mens admin skal ha utvidet tilgang til administrasjon av systemet.

## Teknologistack
- Backend: ASP.NET Core Web API (C#)
- Frontend (enkel visning i dette steget): Razor Pages
- Data: PostgreSQL
- Lokal utvikling: SQLite kan brukes i development
- ORM: Entity Framework Core, inkludert migreringer og seed-data
- Auth: JWT med rollebasert tilgang
- API-dokumentasjon: Swagger/OpenAPI
- Verktøy: Git, GitHub og Docker
- Planlegging og oppgaveflyt: Trello

### Hvorfor Razor i dette steget
Vi har valgt a bruke Razor Pages for denne enkle frontend-delen fordi det er en teknologi vi har jobbet med i et tidligere emne, og vi onsker a vise at vi behersker den i praksis ogsa i dette prosjektet.

## JWT-token / Auth (notater)
Prosjektet bruker .NET 8 (`net8.0`), så JWT-pakken må matche .NET 8.

- Pakkereferanse: `Microsoft.AspNetCore.Authentication.JwtBearer`
- Anbefalt versjon (for .NET 8): `8.0.x`
- Kommando (eksempel):
  - `dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.1`

Merk: Hvis man installerer en for ny versjon (f.eks. 10.x), kan man få feil som `NU1202` (ikke kompatibel med `net8.0`).

## Arbeidsmetode
Teamet bruker Agile som rammeverk for a strukturere arbeidet. Samtidig har vi valgt en fleksibel tilnarming til roller, og fordeler oppgaver uten faste Scrum-roller.

Trello brukes som et visuelt arbeidsverktoy for a organisere oppgaver, folge fremdrift og skape oversikt over hva som skal gjores, hva som er under arbeid og hva som er ferdigstilt. Dette bidrar til bedre samarbeid, tydeligere prioriteringer og mer transparens i prosessen.

## Team og ansvarsfordeling
- Semir  (1) - Auth/Security: JWT, roller (`Admin`/`User`) og tilgangsregler
- Ashwaq (2) - DB/EF Core: entities, migreringer, seed-data og constraints
- Ikram  (3) - Availability/Booking logic: timeslots, opprette og avbestille booking, samt forebygging av dobbeltbooking
- Marion (4) - Testing/Docs/DevOps: integrasjonstester, Swagger-polish, README og CI/Docker

## Mitt ansvarsomrade
Min del av prosjektet er database og EF Core.

Dette inkluderer:
- entities og domenemodeller
- migreringer
- seed-data
- constraints og regler i datamodellen

I praksis betyr det ansvar for hvordan dataene struktureres, hvordan relasjonene mellom tabellene bygges opp, hvordan databasen opprettes og oppdateres gjennom migreringer, og hvordan vi sikrer at dataene folger riktige regler og begrensninger.

## Status
Status per 25.03.2026.

Dette er gjort sa langt:
- ASP.NET Core Web API-prosjekt er opprettet pa .NET 8
- Swagger/OpenAPI er satt opp
- Domene-modeller er opprettet:
  - `AppUser`
  - `MeetingRoom`
  - `Booking`
- Rolle-enum for `Admin` og `User` er laget
- `MeetSlotDbContext` er satt opp og registrert i `Program.cs`
- EF Core og PostgreSQL-provider er lagt til i prosjektet
- Viktige database-regler er konfigurert:
  - unik e-post for brukere
  - unikt navn for møterom
  - `Capacity > 0`
  - `EndTime > StartTime`
  - `DeleteBehavior.Restrict` pa relasjoner fra booking
- Booking-tider lagres i UTC for konsistent tidshandtering
- Forste migrering er opprettet: `MeetSlot_v1`
- Migreringen er kjort mot PostgreSQL-databasen `meetslot`
- Seed-data for `AppUser` inkluderer nå både admin (`admin@meetslot.local`) og standardbruker (`user@meetslot.local`)
- Lokal connection string er flyttet til user-secrets i stedet for a ligge hardkodet i repoet
- Prosjektet bygger uten errors

Dette gjenstar:
- implementere seed-data for rom, brukere og eksempelbookinger
- implementere JWT-autentisering og autorisasjon
- lage rollebaserte API-endepunkter for rom, tilgjengelighet, booking og avbooking
- lage logikk for a forhindre dobbeltbooking i bookingflyten
- erstatte standard `weatherforecast`-endepunkt med faktisk MeetSlot-funksjonalitet
- lage Docker-oppsett
- legge til tester og forbedre API-dokumentasjon videre

## Neste steg
1. Implementere videre seed-data for rom, brukere og eksempelbookinger. Jeg fyller databasen med litt testdata automatisk, slik at teamet har noe a jobbe mot. Admin- og standardbruker er lagt inn, neste er flere eksempelbookinger.
2. Klargjore datalaget videre for bookinglogikk og tilgjengelighet
3. Implementere JWT-autentisering og rollebasert tilgang
4. Lage endepunkter for rom, tilgjengelighet, booking og avbestilling
5. Legge til integrasjonstester og videre Swagger-polish
6. Sette opp Docker og CI nar resten av API-flyten er pa plass

## Merknad
Denne README-en er ment som en arbeidsfil for teamet underveis i utviklingen. Den kan senere erstattes med en mer formell README for innlevering, dokumentasjon eller publisering pa GitHub.






# Student 3 – Timeslots, booking create/cancel, double-booking prevention

## Opprettelse av DTO (CreateBookingDto)

Før booking-endepunktet ble implementert, ble det opprettet en egen DTO-klasse kalt `CreateBookingDto`.

DTO står for **Data Transfer Object** og brukes for å kontrollere hvilke data klienten sender inn til API-et.

I stedet for å sende hele `Booking`-entiteten direkte, ble DTO brukt for å begrense input til kun nødvendige felt:

* Title
* StartTime
* EndTime
* MeetingRoomId
* AppUserId

Dette ble gjort for å:

* unngå at klienten sender navigasjonsegenskaper som `MeetingRoom` og `AppUser`
* redusere risiko for serialiseringsproblemer
* gjøre input tydeligere i Swagger

DTO-filen ble opprettet i prosjektmappen:    `Dtos/CreateBookingDto.cs`

---

## 1. POST - endepunkt for booking  `POST /api/Booking`

Et `BookingController` ble opprettet for å håndtere bookinglogikken.

Første implementasjon av `POST /api/Booking` gjorde følgende:

* mottok bookingdata via `CreateBookingDto`
* hentet valgt møterom fra databasen
* hentet valgt bruker fra databasen
* opprettet et nytt `Booking`-objekt
* lagret bookingen med `_context.SaveChanges()`

---

### 1. Validering lagt til i booking-endepunktet

Etter første test ble flere kontroller lagt til:

#### Kontroll av møterom

API-et sjekker om `MeetingRoomId` finnes i databasen.

Hvis rommet ikke finnes: `BadRequest("Møterommet ble ikke funnet")`

#### Kontroll av bruker

API-et sjekker om `AppUserId` finnes i databasen.

Hvis brukeren ikke finnes: `BadRequest("Brukeren ble ikke funnet")`

#### Kontroll av tid

API-et sjekker at sluttidspunkt er etter starttidspunkt.

Hvis `EndTime <= StartTime`:
**BadRequest("Slutttidspunkt må være etter starttidspunkt")**

---

### 2. Forebygging av dobbeltbooking

Det ble lagt inn logikk for å forhindre at samme møterom bookes i overlappende tidsrom.

Kontrollen ble gjort med:

```csharp
var hasConflict = _context.Bookings.Any(b =>
    b.MeetingRoomId == dto.MeetingRoomId &&
    dto.StartTime < b.EndTime &&
    dto.EndTime > b.StartTime);
```

Hvis konflikten finnes:

`BadRequest("Dette rommet er allerede booket for det valgte tidspunktet.")`

Denne logikken sikrer at to bookinger ikke kan overlappe i samme møterom.

---


### 3. Testing i Swagger

Endepunktet ble testet i Postman og Swagger med JSON-request.
![POST](./1.POST.png)

#### Gyldig booking

- Booking ble lagret i databasen og verifisert i DBeaver. `200 OK`

#### Ugyldig booking

- Ved overlappende tidspunkt returnerte API-et: `400 Bad Request`

med melding:

`Dette rommet er allerede booket for det valgte tidspunktet.`

Dette bekrefter at booking-logikken fungerer som forventet.



## 2. DELETE - endepunkt for booking  `DELETE /api/Booking/{id}`

Dette endpointet brukes for å slette en eksisterende booking fra databasen.  

Endpointen søker først etter booking med gitt id.  
Hvis booking ikke finnes, returneres en feil (`404 NotFound`).  
Hvis booking finnes, fjernes den fra databasen og endringene lagres.  

**Route:** `DELETE /api/Booking/{id}`

**Eksempel:** `DELETE /api/Booking/7`
![DELETE](2.DELETE.png)

**Resultat:**
- booking slettes hvis den finnes
- feil returneres hvis booking ikke eksisterer



## 3. GET - endepunkt for booking  `GET /api/Booking`

Dette endpointet brukes for å hente alle bookinger fra databasen.

Systemet leser alle booking-objekter og returnerer dem som en liste.

**Route:** `GET /api/Booking`
![GET](3.GET.png)

**Resultat:**
- returnerer alle registrerte bookinger



## 4. GET - endepunkt for booking  `GET /api/Booking/{id}`
Dette endpointet brukes for å hente én booking basert på id.

Systemet søker etter booking i databasen.  
Hvis booking finnes, returneres objektet.  
Hvis booking ikke finnes, returneres `404 NotFound`.  

**Route:** `GET /api/Booking/{id}`

**Eksempel:** `GET /api/Booking/5`
![GET](4.GET:{id}.png)
**Resultat:**
- returnerer valgt booking hvis den finnes
- feil hvis booking ikke eksisterer

## 5. GET - endepunkt for booking  `GET /api/Booking/available-slots?meetingRoomId={id}&date={yyyy-MM-dd}`

### Available slots endpoint
Dette endpointet brukes for å hente ledige tider for et møterom på en valgt dato.

Systemet oppretter først mulige timeslots innenfor arbeidstiden.
Deretter sammenlignes disse med eksisterende bookinger for å finne hvilke slots som er ledige.

**Route:** `GET /api/Booking/available-slots?meetingRoomId={id}&date={yyyy-MM-dd}`

**Eksempel:** `GET /api/Booking/available-slots?meetingRoomId=1&date=2026-04-30`
![Available](5.Available.png)
**Resultat:**
- returnerer ledige tider for valgt møterom
- returnerer feil hvis møterommet ikke finnes


## Steg: Enkel Razor Pages frontend lokalt (`/rooms`)

For dette steget lager vi en enkel, men mer visuell frontend-visning i samme prosjekt, slik at vi kan se møterommene direkte i nettleseren uten a bygge hele frontend-stacken først.

Målet er:
- en fungerende side pa `https://localhost:{PORT}/rooms`
- siden henter data fra databasen via `MeetSlotDbContext`
- viser møterom med navn, kapasitet og beskrivelse i et kort-basert layout

Dette steget er ment for lokal utvikling og rask verifisering av dataflyt i prosjektet.

### Steg 1 - Oppdater `Program.cs`

Legg til `AddRazorPages()` sammen med de andre services:

```csharp
builder.Services.AddRazorPages();
```

Legg til `MapRazorPages()` rett før `app.Run()`:

```csharp
app.MapRazorPages();
```

Status na: dette er allerede lagt inn i `Program.cs`.

### Steg 2 - Lag `Pages`-mappen og filene

Opprett:
- `Pages/Rooms.cshtml`
- `Pages/Rooms.cshtml.cs`

I dette prosjektet er disse filene allerede opprettet lokalt.

### Steg 3 - Hent møterom fra databasen

I `Pages/Rooms.cshtml.cs` hentes data med `MeetSlotDbContext` fra `_context.MeetingRooms`, og resultatet vises i `Pages/Rooms.cshtml` med:
- navn
- kapasitet
- beskrivelse

I viewet er presentasjonen oppdatert med:
- toppseksjon (hero) med antall rom og total kapasitet
- grid av romkort for bedre oversikt
- enkel lokal styling for rask prototype-visning

### Steg 4 - Direkte routing til `/rooms`

Vi sender brukeren direkte til `/rooms` fra rot-url (`/`) fordi dette er den eneste frontend-siden i dette steget, og den representerer hovedfunksjonen vi vil demonstrere.

Dette gir tre fordeler:
- unngar tom/hvit side på `/` under lokal testing
- gjør demo-flyten enklere for teamet (appen åpner rett sted)
- samsvarer med `launchSettings` som nå bruker `rooms` som `launchUrl`

Implementert i `Program.cs`:

```csharp
app.MapGet("/", () => Results.Redirect("/rooms"));
```

### Steg 5 - Filterpills i romoversikt

For a gjøre oversikten mer brukervennlig er det lagt til filterpills i `Rooms.cshtml`.

Brukeren kan filtrere rom visuelt etter kapasitet:
- Alle
- Liten (1-4)
- Medium (5-8)
- Stor (9+)

Filteret er laget med enkel JavaScript + CSS i samme Razor-side, uten ny backend-logikk.
Hvert romkort har `data-capacity` slik at filteret kan vise/skjule kort lokalt i nettleseren.

### Steg 6 - Book-knapp og `ComingSoon`-side (under utvikling)

Det er opprettet en egen Razor Page for "under utvikling":
- `Pages/ComingSoon.cshtml`
- `Pages/ComingSoon.cshtml.cs`

Siden viser en tydelig og vennlig melding fram til bookingflyten er ferdig koblet, og fungerer som midlertidig målside for booking-knapper.

Status na:
- `ComingSoon`-siden er implementert og bygger uten feil
- neste kobling er a sende "Book" fra romkortene til `/comingsoon`

### Kort test lokalt

1. Kjør prosjektet lokalt (`dotnet run` eller fra VS Code)
2. Åpne nettleser pa `https://localhost:{PORT}/rooms`
3. Verifiser at listen viser møterom fra databasen

Hvis listen er tom, sjekk at databasen er migrert og at seed-data for `MeetingRooms` finnes.






# Docker-oppsett

## 1. Bytt til riktig branch
Prosjektet kan kjøres lokalt med Docker ved hjelp av `Dockerfile` og `docker-compose.yml`. 

```bash
git checkout main     
git pull origin main
git checkout -b docker-setup
```  


## 2. Bygg og start containerne
```bash
docker compose up --build
```
Applikasjonen kjører på:   http://localhost:5182

## 3. Kjør Entity Framework migrations
Første gang containerne startes, er databasen tom. Hvis du får en feil som:
```bash
relation "MeetingRooms" does not exist
```
kjør EF Core migrations fra prosjektmappen:
```bash
ConnectionStrings__DefaultConnection="Host=localhost;Port=5433;Database=meetslot;Username=postgres;Password=postgres" dotnet ef database update
```
Dette oppretter databasetabellene og legger inn seed-data.

## 4. Test applikasjonen
Åpne UI Pages-siden:    http://localhost:5182/rooms  
Test Booking API-et:    http://localhost:5182/api/Booking  
Hvis det ikke finnes bookinger ennå, skal responsen være:
```bash
[]
```
Åpne Swagger:          http://localhost:5182/swagger  

## 5. Testing alle endepunkter
Eksempler på endepunkter:
- GET     /api/Booking
- GET     /api/Booking/{id}
- POST    /api/Booking
- DELETE  /api/Booking/{id}
- GET     /api/Booking/available-slots
- POST    /api/Auth/register
- POST    /api/Auth/login



## 6. Stopp Docker
```bash
docker compose down
```