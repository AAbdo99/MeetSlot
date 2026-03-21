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
- Data: PostgreSQL
- Lokal utvikling: SQLite kan brukes i development
- ORM: Entity Framework Core, inkludert migreringer og seed-data
- Auth: JWT med rollebasert tilgang
- API-dokumentasjon: Swagger/OpenAPI
- Verktøy: Git, GitHub og Docker
- Planlegging og oppgaveflyt: Trello

## Arbeidsmetode
Teamet bruker Agile som rammeverk for a strukturere arbeidet. Samtidig har vi valgt en fleksibel tilnarming til roller, og fordeler oppgaver uten faste Scrum-roller.

Trello brukes som et visuelt arbeidsverktoy for a organisere oppgaver, folge fremdrift og skape oversikt over hva som skal gjores, hva som er under arbeid og hva som er ferdigstilt. Dette bidrar til bedre samarbeid, tydeligere prioriteringer og mer transparens i prosessen.

## Team og ansvarsfordeling
- Ikram (1) - Auth/Security: JWT, roller (`Admin`/`User`) og tilgangsregler
- Ashwaq (2) - DB/EF Core: entities, migreringer, seed-data og constraints
- Semir (3) - Availability/Booking logic: timeslots, opprette og avbestille booking, samt forebygging av dobbeltbooking
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
Status per 19.03.2026.

Dette er gjort sa langt:
- ASP.NET Core Web API-prosjekt er opprettet pa .NET 8
- Swagger/OpenAPI er satt opp
- Domene-modeller er opprettet:
  - `AppUser`
  - `MeetingRoom`
  - `Booking`
- Rolle-enum for `Admin` og `User` er laget
- Prosjektet bygger uten errors

Dette gjenstar:
- opprette `DbContext` og bygge datalaget i `Data/`
- legge til EF Core-pakker, migreringer og seed-data
- sette opp PostgreSQL som maldatabase
- eventuelt bruke SQLite for lokal utvikling
- implementere JWT-autentisering og autorisasjon
- lage rollebaserte API-endepunkter for rom, tilgjengelighet, booking og avbooking
- erstatte standard `weatherforecast`-endepunkt med faktisk MeetSlot-funksjonalitet
- lage Docker-oppsett

## Neste steg
1. Opprette `ApplicationDbContext` og registrere den i `Program.cs`
2. Legge til EF Core og riktig databaseprovider, primart Npgsql
3. Lage forste migrering og koble prosjektet mot database
4. Implementere seed-data for rom, brukere og eksempelbookinger
5. Definere constraints for relasjoner, obligatoriske felt og datavalidering
6. Klargjore datalaget for videre arbeid med autentisering og bookinglogikk

## Merknad
Denne README-en er ment som en arbeidsfil for teamet underveis i utviklingen. Den kan senere erstattes med en mer formell README for innlevering, dokumentasjon eller publisering pa GitHub.
