FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

WORKDIR /app

COPY Difficalcy.sln .
COPY Difficalcy/Difficalcy.csproj ./Difficalcy/
COPY Difficalcy.Catch/Difficalcy.Catch.csproj ./Difficalcy.Catch/
COPY Difficalcy.Catch.Tests/Difficalcy.Catch.Tests.csproj ./Difficalcy.Catch.Tests/
COPY Difficalcy.Mania/Difficalcy.Mania.csproj ./Difficalcy.Mania/
COPY Difficalcy.Mania.Tests/Difficalcy.Mania.Tests.csproj ./Difficalcy.Mania.Tests/
COPY Difficalcy.Osu/Difficalcy.Osu.csproj ./Difficalcy.Osu/
COPY Difficalcy.Osu.Tests/Difficalcy.Osu.Tests.csproj ./Difficalcy.Osu.Tests/
COPY Difficalcy.Taiko/Difficalcy.Taiko.csproj ./Difficalcy.Taiko/
COPY Difficalcy.Taiko.Tests/Difficalcy.Taiko.Tests.csproj ./Difficalcy.Taiko.Tests/
COPY Difficalcy.Tests/Difficalcy.Tests.csproj ./Difficalcy.Tests/

RUN dotnet restore

COPY . .
