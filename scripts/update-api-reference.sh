#!/bin/bash
set -euo pipefail

dotnet build

cp Difficalcy.Api/obj/Difficalcy.Api.json docs/docs/api-reference/difficalcy.json
