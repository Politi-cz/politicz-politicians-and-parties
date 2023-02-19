FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY politicz-politicians-and-parties/PoliticiansAndParties.Api.csproj .
RUN dotnet restore
COPY politicz-politicians-and-parties .
RUN dotnet publish "PoliticiansAndParties.Api.csproj" -c Release -o /publish /p:UseAppHost=false
RUN dotnet dev-certs https

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /publish .
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
EXPOSE 80 443
ENTRYPOINT ["/bin/sh", "-c", "echo 'Waiting for other services to start up' && sleep 15 && dotnet PoliticiansAndParties.Api.dll"]
