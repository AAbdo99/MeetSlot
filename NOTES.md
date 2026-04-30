# MeetSlot – Utviklingsnotater

## Prosjektoppsett
- .NET versjon: `net8.0`
- Database: PostgreSQL 14 (Homebrew)
- ORM: Entity Framework Core 8

## Viktig om pakkeversjonering
Prosjektet bruker .NET 8, så alle pakker fra `Microsoft.AspNetCore.*` må matche versjon 8.x.
Bruk alltid `--version 8.0.x` når du installerer slike pakker, ellers får du `NU1202`-feil.

Eksempel:
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.1

## Databaseoppsett (lokalt)
1. Kopier `appsettings.Example.json` og gi den nytt navn til `appsettings.json`
2. Fyll inn ditt eget PostgreSQL-brukernavn og passord
3. Kjør `dotnet ef database update` for å opprette tabeller og seed-data

## Migrasjoner
- `MeetSlot_v1` – Initial oppsett av entiteter
- `UpdateConstraintsAndSeed` – Constraints, UTC-håndtering og seed-data
- `AddTitleAndCreatedAtUtcToBooking` – La til Title og CreatedAtUtc på Booking