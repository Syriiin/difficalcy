FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS base

LABEL org.opencontainers.image.source https://github.com/Syriiin/difficalcy

WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV BEATMAP_DIRECTORY=/app/beatmaps

FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /src

COPY ./Difficalcy/Difficalcy.csproj ./Difficalcy/
COPY ./Difficalcy.Taiko/Difficalcy.Taiko.csproj ./Difficalcy.Taiko/
RUN dotnet restore Difficalcy.Taiko/Difficalcy.Taiko.csproj

COPY ./Difficalcy/ ./Difficalcy/
COPY ./Difficalcy.Taiko/ ./Difficalcy.Taiko/
RUN dotnet publish Difficalcy.Taiko/Difficalcy.Taiko.csproj -o /app/build --runtime linux-x64 --self-contained false

FROM base AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "Difficalcy.Taiko.dll"]
