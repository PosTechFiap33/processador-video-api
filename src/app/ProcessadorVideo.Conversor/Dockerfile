
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production

# Atualizar pacotes e instalar FFmpeg
RUN apt-get update && \
    apt-get install -y ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "ProcessadorVideo.Conversor/ProcessamentoVideo.Conversor.Worker/ProcessamentoVideo.Conversor.Worker.csproj"
COPY . .
WORKDIR "/src/ProcessadorVideo.Conversor/ProcessamentoVideo.Conversor.Worker"
RUN dotnet build "ProcessamentoVideo.Conversor.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProcessamentoVideo.Conversor.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProcessamentoVideo.Conversor.Worker.dll"]
