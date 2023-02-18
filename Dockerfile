#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["politicz-politicians-and-parties/PoliticiansAndParties.Api.csproj", "politicz-politicians-and-parties/"]
RUN dotnet restore "politicz-politicians-and-parties/PoliticiansAndParties.Api.csproj"
COPY . .
WORKDIR "/src/politicz-politicians-and-parties"
RUN dotnet build "PoliticiansAndParties.Api.csproj" -c Release -o /app/build
RUN dotnet dev-certs https


FROM build AS publish
RUN dotnet publish "PoliticiansAndParties.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
ENTRYPOINT ["/bin/sh", "-c", "echo 'Waiting for DB to start up' && sleep 20 && dotnet PoliticiansAndParties.Api.dll"]
