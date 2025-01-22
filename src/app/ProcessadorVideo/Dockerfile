
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443
EXPOSE 5001

# Atualizar pacotes e instalar FFmpeg
RUN apt-get update && \
    apt-get install -y ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "adapter/ProcessadorVideo.Api/ProcessadorVideo.Api.csproj"
COPY . .
WORKDIR "/src/adapter/ProcessadorVideo.Api"
RUN dotnet build "ProcessadorVideo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProcessadorVideo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProcessadorVideo.Api.dll"]
