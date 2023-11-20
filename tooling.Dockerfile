FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base

WORKDIR /app

COPY Difficalcy.sln .
COPY Difficalcy/Difficalcy.csproj ./Difficalcy/
COPY Difficalcy.Catch/Difficalcy.Catch.csproj ./Difficalcy.Catch/
COPY Difficalcy.Mania/Difficalcy.Mania.csproj ./Difficalcy.Mania/
COPY Difficalcy.Osu/Difficalcy.Osu.csproj ./Difficalcy.Osu/
COPY Difficalcy.Taiko/Difficalcy.Taiko.csproj ./Difficalcy.Taiko/
COPY Difficalcy.Tests/Difficalcy.Tests.csproj ./Difficalcy.Tests/

RUN dotnet restore

COPY . .
