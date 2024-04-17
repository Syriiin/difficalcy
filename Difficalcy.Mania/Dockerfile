FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

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
RUN dotnet restore Difficalcy/Difficalcy.csproj
COPY ./Difficalcy.Mania/Difficalcy.Mania.csproj ./Difficalcy.Mania/
RUN dotnet restore Difficalcy.Mania/Difficalcy.Mania.csproj

COPY ./Difficalcy/ ./Difficalcy/
COPY ./Difficalcy.Mania/ ./Difficalcy.Mania/
RUN dotnet build Difficalcy.Mania/Difficalcy.Mania.csproj -c Release -o /app/build

FROM base AS final
WORKDIR /app
COPY --from=build /app/build .
RUN mkdir beatmaps
ENTRYPOINT ["dotnet", "Difficalcy.Mania.dll"]
