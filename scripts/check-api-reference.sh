#!/bin/bash
set -euo pipefail

diff -q docs/docs/api-reference/difficalcy-osu.json <(curl --silent localhost:5000/swagger/v1/swagger.json)
diff -q docs/docs/api-reference/difficalcy-taiko.json <(curl --silent localhost:5001/swagger/v1/swagger.json)
diff -q docs/docs/api-reference/difficalcy-catch.json <(curl --silent localhost:5002/swagger/v1/swagger.json)
diff -q docs/docs/api-reference/difficalcy-mania.json <(curl --silent localhost:5003/swagger/v1/swagger.json)
