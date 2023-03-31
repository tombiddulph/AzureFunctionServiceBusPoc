FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated7.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

COPY ../docker/rabbitmq-amqp1/testca/cacert.pem /usr/local/share/ca-certificates/cacert.crt
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Functions.ServiceBus/Functions.ServiceBus.csproj", "Functions.ServiceBus/"]
RUN dotnet restore "Functions.ServiceBus/Functions.ServiceBus.csproj"
COPY . .
WORKDIR "/src/Functions.ServiceBus"
RUN dotnet build "Functions.ServiceBus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Functions.ServiceBus.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
    EmulatorConnectionString="Endpoint=sb://sbemulator/;SharedAccessKeyName=all;SharedAccessKey=CLwo3FQ3S39Z4pFOQDefaiUd1dSsli4XOAj3Y9Uh1E=;EnableAmqpLinkRedirect=false"