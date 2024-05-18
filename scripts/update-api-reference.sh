#!/bin/bash
set -euo pipefail

curl localhost:5000/swagger/v1/swagger.json -o docs/docs/api-reference/difficalcy-osu.json
curl localhost:5001/swagger/v1/swagger.json -o docs/docs/api-reference/difficalcy-taiko.json
curl localhost:5002/swagger/v1/swagger.json -o docs/docs/api-reference/difficalcy-catch.json
curl localhost:5003/swagger/v1/swagger.json -o docs/docs/api-reference/difficalcy-mania.json
