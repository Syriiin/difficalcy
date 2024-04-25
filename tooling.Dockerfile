FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS base

RUN curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg \
    && chmod go+r /usr/share/keyrings/githubcli-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
    && apt update \
    && apt install gh -y

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
