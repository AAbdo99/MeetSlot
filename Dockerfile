FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MeetSlot.csproj ./
RUN dotnet restore MeetSlot.csproj

COPY . ./
RUN dotnet publish MeetSlot.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "MeetSlot.dll"]