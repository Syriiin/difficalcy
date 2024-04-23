# difficalcy-taiko

## `GET /api/calculator/info`

### Response

```json
{
  "rulesetName": "string",
  "calculatorName": "string",
  "calculatorPackage": "string",
  "calculatorVersion": "string",
  "calculatorUrl": "string"
}
```

## `GET /api/calculator/difficulty`

### Query Parameters

| Name      | Description | Required | Type    |
| --------- | ----------- | -------- | ------- |
| Accuracy  |             | No       | double  |
| Combo     |             | No       | integer |
| Misses    |             | No       | integer |
| Oks       |             | No       | integer |
| BeatmapId |             | Yes      | string  |
| Mods      |             | No       | integer |

### Response

```json
{
  "total": 0,
  "stamina": 0,
  "rhythm": 0,
  "colour": 0
}
```

## `GET /api/calculator/performance`

### Query Parameters

| Name      | Description | Required | Type    |
| --------- | ----------- | -------- | ------- |
| Accuracy  |             | No       | double  |
| Combo     |             | No       | integer |
| Misses    |             | No       | integer |
| Oks       |             | No       | integer |
| BeatmapId |             | Yes      | string  |
| Mods      |             | No       | integer |

### Response

```json
{
  "total": 0,
  "difficulty": 0,
  "accuracy": 0
}
```

## `GET /api/calculator/calculation`

### Query Parameters

| Name      | Description | Required | Type    |
| --------- | ----------- | -------- | ------- |
| Accuracy  |             | No       | double  |
| Combo     |             | No       | integer |
| Misses    |             | No       | integer |
| Oks       |             | No       | integer |
| BeatmapId |             | Yes      | string  |
| Mods      |             | No       | integer |

### Response

```json
{
  "difficulty": {
    "total": 0,
    "stamina": 0,
    "rhythm": 0,
    "colour": 0
  },
  "performance": {
    "total": 0,
    "difficulty": 0,
    "accuracy": 0
  }
}
```

## `POST /api/calculator/batch/difficulty`

### Body Parameters

```json
[
  {
    "beatmapId": "string",
    "mods": 0,
    "accuracy": 0,
    "combo": 0,
    "misses": 0,
    "oks": 0
  }
]
```

### Response

```json

  {
    "total": 0,
    "stamina": 0,
    "rhythm": 0,
    "colour": 0
  }
]
```

## `POST /api/calculator/batch/performance`

### Body Parameters

```json
[
  {
    "beatmapId": "string",
    "mods": 0,
    "accuracy": 0,
    "combo": 0,
    "misses": 0,
    "oks": 0
  }
]
```

### Response

```json
[
  {
    "total": 0,
    "difficulty": 0,
    "accuracy": 0
  }
]
```

## `POST /api/calculator/batch/calculation`

### Body Parameters

```json
[
  {
    "beatmapId": "string",
    "mods": 0,
    "accuracy": 0,
    "combo": 0,
    "misses": 0,
    "oks": 0
  }
]
```

### Response

```json
[
  {
    "difficulty": {
      "total": 0,
      "stamina": 0,
      "rhythm": 0,
      "colour": 0
    },
    "performance": {
      "total": 0,
      "difficulty": 0,
      "accuracy": 0
    }
  }
]
```
