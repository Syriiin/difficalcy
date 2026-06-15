#!/bin/bash
set -euo pipefail

dotnet build

cp Difficalcy.Osu/obj/Difficalcy.Osu.json docs/docs/api-reference/difficalcy-osu.json
cp Difficalcy.Taiko/obj/Difficalcy.Taiko.json docs/docs/api-reference/difficalcy-taiko.json
cp Difficalcy.Catch/obj/Difficalcy.Catch.json docs/docs/api-reference/difficalcy-catch.json
cp Difficalcy.Mania/obj/Difficalcy.Mania.json docs/docs/api-reference/difficalcy-mania.json
