# API Reference

difficalcy exposes calculators for all official rulesets in a single server.

Each calculator is accessible by key under `/api/calculators/{key}`:

| Endpoint                                           | Description                                           |
| -------------------------------------------------- | ----------------------------------------------------- |
| `GET /api/calculators`                             | Lists all available calculators with their keys       |
| `GET /api/calculators/{key}/info`                  | Returns details of the calculation engine             |
| `GET /api/calculators/{key}/calculation`           | Calculates both difficulty and performance of a score |
| `POST /api/calculators/{key}/batch/calculation`    | Calculates a batch of difficulties and performances   |
| `GET /api/calculators/{key}/beatmapdetails`        | Returns beatmap metadata and statistics               |

The default available keys are `osu`, `taiko`, `catch`, and `mania`.

---

[OAD(./docs/api-reference/difficalcy.json)]

---

Generated from [difficalcy OpenAPI schema](./difficalcy.json)
