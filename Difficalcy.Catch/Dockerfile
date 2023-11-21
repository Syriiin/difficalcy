FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

LABEL org.opencontainers.image.source https://github.com/Syriiin/difficalcy

WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./Difficalcy/Difficalcy.csproj ./Difficalcy/
RUN dotnet restore Difficalcy/Difficalcy.csproj
COPY ./Difficalcy.Catch/Difficalcy.Catch.csproj ./Difficalcy.Catch/
RUN dotnet restore Difficalcy.Catch/Difficalcy.Catch.csproj

COPY ./Difficalcy/ ./Difficalcy/
COPY ./Difficalcy.Catch/ ./Difficalcy.Catch/
RUN dotnet build Difficalcy.Catch/Difficalcy.Catch.csproj -c Release -o /app/build

FROM base AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "Difficalcy.Catch.dll"]
