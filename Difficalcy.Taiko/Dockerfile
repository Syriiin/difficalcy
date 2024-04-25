FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.19 AS base

LABEL org.opencontainers.image.source https://github.com/Syriiin/difficalcy

WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV BEATMAP_DIRECTORY=/app/beatmaps

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./Difficalcy/Difficalcy.csproj ./Difficalcy/
COPY ./Difficalcy.Taiko/Difficalcy.Taiko.csproj ./Difficalcy.Taiko/
RUN dotnet restore Difficalcy.Taiko/Difficalcy.Taiko.csproj

COPY ./Difficalcy/ ./Difficalcy/
COPY ./Difficalcy.Taiko/ ./Difficalcy.Taiko/
RUN dotnet build Difficalcy.Taiko/Difficalcy.Taiko.csproj -c Release -o /app/build

FROM base AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "Difficalcy.Taiko.dll"]
