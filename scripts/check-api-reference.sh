#!/bin/bash
set -euo pipefail

dotnet build

diff -q docs/docs/api-reference/difficalcy-osu.json Difficalcy.Osu/obj/Difficalcy.Osu.json
diff -q docs/docs/api-reference/difficalcy-taiko.json Difficalcy.Taiko/obj/Difficalcy.Taiko.json
diff -q docs/docs/api-reference/difficalcy-catch.json Difficalcy.Catch/obj/Difficalcy.Catch.json
diff -q docs/docs/api-reference/difficalcy-mania.json Difficalcy.Mania/obj/Difficalcy.Mania.json
