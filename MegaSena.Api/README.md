# MegaSena Prediction API

REST API for generating MegaSena lottery predictions based on cycle analysis and historical frequency patterns.

## 🚀 Quick Start

### Run the API

```bash
dotnet run --project MegaSena.Api
```

The API will start on `http://localhost:5028` (or the port specified in launchSettings.json).

### Access Swagger UI

Open your browser and navigate to:
```
http://localhost:5028
```

The Swagger UI will be displayed at the root URL, allowing you to explore and test all available endpoints.

## 📡 API Endpoints

### 1. Get Next Draw Prediction

**Endpoint:** `GET /api/predictions/next-draw`

**Description:** Returns betting recommendations and scenarios for the next MegaSena draw based on cycle analysis.

**Response Example:**
```json
{
  "remainingNumbers": [3, 7, 12, 18, 25, 33, 41, 47, 52, 58],
  "totalRemaining": 10,
  "frequencyDistribution": {
    "5": [1, 4, 9, 15],
    "4": [2, 8, 11, 19],
    "3": [5, 13, 22, 28]
  },
  "recommendedBets": [
    {
      "numbers": [3, 8, 15, 22, 28, 41],
      "strategy": "Mid-range frequency (2-4 times)",
      "remainingNumberIncluded": 3,
      "formattedBet": "3-8-15-22-28-41"
    }
  ],
  "scenarios": [
    {
      "remainingNumber": 3,
      "strategies": [
        {
          "strategyName": "Most Common",
          "description": "Numbers drawn 5 times",
          "numbers": [1, 3, 4, 9, 15, 20],
          "formattedBet": "1-3-4-9-15-20"
        }
      ]
    }
  ],
  "currentCycle": {
    "startDate": "2024-10-15T00:00:00",
    "lastConcurso": 2940,
    "drawsInCycle": 45,
    "daysSinceStart": 33
  }
}
```

### 2. Health Check

**Endpoint:** `GET /api/health`

**Description:** Returns the health status of the API.

**Response Example:**
```json
{
  "status": "healthy",
  "timestamp": "2024-11-17T12:34:56.789Z"
}
```

## 🏗️ Architecture

### Project Structure

```
MegaSena.Api/
├── Models/              # DTOs for API responses
│   └── PredictionResponse.cs
├── Services/            # Business logic services
│   └── PredictionService.cs
├── Program.cs           # API configuration and endpoints
└── MegaSena.Api.csproj  # Project file
```

### Dependencies

- **ASP.NET Core 9.0** - Web API framework
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI documentation
- **MegaSena** - Core lottery analysis library (project reference)

### Design Patterns

- **Dependency Injection** - Services registered in DI container
- **DTOs (Data Transfer Objects)** - Separate models for API responses
- **Minimal API** - Modern ASP.NET Core endpoint routing
- **Separation of Concerns** - API layer separate from business logic

## 🔧 Configuration

### Launch Settings

The API uses `Properties/launchSettings.json` for local development configuration:

- **HTTP Port:** 5028
- **HTTPS Port:** 7028 (if configured)
- **Environment:** Development
- **Swagger:** Enabled in Development mode

### Swagger Configuration

- **Swagger UI:** Available at root URL (`/`)
- **Swagger JSON:** Available at `/swagger/v1/swagger.json`
- **XML Documentation:** Enabled for detailed API documentation

## 🧪 Testing

### Using Swagger UI

1. Run the API: `dotnet run --project MegaSena.Api`
2. Open browser: `http://localhost:5028`
3. Click on an endpoint to expand it
4. Click "Try it out"
5. Click "Execute"
6. View the response

### Using curl

```bash
# Get predictions
curl http://localhost:5028/api/predictions/next-draw

# Health check
curl http://localhost:5028/api/health
```

### Using PowerShell

```powershell
# Get predictions
Invoke-WebRequest -Uri http://localhost:5028/api/predictions/next-draw | Select-Object -ExpandProperty Content

# Health check
Invoke-WebRequest -Uri http://localhost:5028/api/health | Select-Object -ExpandProperty Content
```

## 📊 Response Models

### PredictionResponse

Main response model containing all prediction data.

**Properties:**
- `remainingNumbers` - Numbers not yet drawn in current cycle
- `totalRemaining` - Count of remaining numbers
- `frequencyDistribution` - Numbers grouped by draw frequency
- `recommendedBets` - Top betting recommendations
- `scenarios` - Detailed scenarios for each remaining number
- `currentCycle` - Information about the current cycle

### BettingRecommendation

Individual betting recommendation.

**Properties:**
- `numbers` - The 6 numbers to bet (sorted)
- `strategy` - Strategy description
- `remainingNumberIncluded` - Which remaining number is included
- `formattedBet` - Formatted bet string (e.g., "2-12-15-29-41-44")

## 🔮 Future Enhancements

- Add authentication/authorization
- Add rate limiting
- Add caching for predictions
- Add historical prediction tracking
- Add more prediction strategies
- Add support for other lottery games (Quina, Lotofácil, etc.)

