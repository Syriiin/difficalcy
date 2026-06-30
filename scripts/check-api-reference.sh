#!/bin/bash
set -euo pipefail

dotnet build

diff -q docs/docs/api-reference/difficalcy.json Difficalcy.Api/obj/Difficalcy.Api.json
