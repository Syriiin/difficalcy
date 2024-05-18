#!/bin/bash
set -euo pipefail

dotnet build

dotnet tool run swagger tofile Difficalcy.Osu/bin/Debug/net8.0/Difficalcy.Osu.dll v1 > docs/docs/api-reference/difficalcy-osu.json
dotnet tool run swagger tofile Difficalcy.Taiko/bin/Debug/net8.0/Difficalcy.Taiko.dll v1 > docs/docs/api-reference/difficalcy-taiko.json
dotnet tool run swagger tofile Difficalcy.Catch/bin/Debug/net8.0/Difficalcy.Catch.dll v1 > docs/docs/api-reference/difficalcy-catch.json
dotnet tool run swagger tofile Difficalcy.Mania/bin/Debug/net8.0/Difficalcy.Mania.dll v1 > docs/docs/api-reference/difficalcy-mania.json
