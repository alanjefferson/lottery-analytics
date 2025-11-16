# Lottery Analytics

Statistical analysis tool for Brazilian lottery results using cycle-based pattern recognition and predictive modeling.

## Overview

This project provides comprehensive statistical analysis for Brazilian lottery games, currently focusing on **Mega-Sena**. The system uses cycle-based pattern recognition to analyze historical draw data and generate insights about number frequency patterns.

## MegaSena Project

### What is Mega-Sena?

Mega-Sena is Brazil's most popular lottery game where players select 6 numbers from 1 to 60. This project analyzes historical draw data to identify patterns and generate statistical predictions.

### Core Concept: Cycle Analysis

The system tracks **"cycles"** - periods where all 60 numbers (1-60) are drawn at least once. Historical data shows:

- **Average cycle duration**: ~173 days (5.7 months)
- **Median cycle duration**: ~161 days (5.3 months)
- **Range**: 91 to 343 days (3 to 11.3 months)

### Key Features

#### 1. **Cycle Tracking**

- Monitors when all 60 numbers have been drawn at least once
- Tracks cycle start/end dates and duration
- Generates detailed CSV reports for each completed cycle

#### 2. **Frequency Analysis**

- Analyzes how often each number is drawn within cycles
- Groups numbers by frequency (drawn 1x, 2x, 3x, etc.)
- Identifies patterns in number distribution

#### 3. **Prediction System**

- Generates 6-number betting combinations based on:
  - Remaining numbers in current cycle
  - Historical frequency patterns
  - Statistical probability distributions
- Provides multiple betting strategies:
  - **Strategy #1**: Most common frequency (3 times - 22.7% probability)
  - **Strategy #2**: Mid-range frequency (2-4 times - 62.5% combined probability)
  - **Strategy #3**: Balanced approach (average frequency ~3.08)

#### 4. **Multiple Analysis Tools**

- Position pattern analysis
- Even/odd distribution analysis
- Number range analysis
- Spacing and gap analysis
- Cycle duration statistics

### Technology Stack

- **Language**: C# (.NET 6.0)
- **Data Source**: Excel file (`MegaSena/Input/Mega-Sena.xlsx`)
- **Output**: CSV files in `Output/MegaSena/`
- **Dependencies**: DocumentFormat.OpenXml (for Excel reading)

### Project Structure

```
lottery-analytics/
├── MegaSena/
│   ├── Analysis/              # Analysis modules
│   │   ├── PredictNextDraw.cs
│   │   ├── AnalyzeCycleDuration.cs
│   │   ├── AnalyzeThreeNumbersRemaining.cs
│   │   └── ...
│   ├── Core/                  # Core functionality
│   │   ├── MegaSenaResults.cs
│   │   ├── CycleResults.cs
│   │   └── CycleResultsWriter.cs
│   ├── Entity/                # Data models
│   │   ├── MegaSenaDraw.cs
│   │   ├── Cycle.cs
│   │   └── CycleNumber.cs
│   ├── Models/                # Supporting models
│   ├── Input/                 # Input data files
│   │   └── Mega-Sena.xlsx
│   └── Program.cs             # Main entry point
├── Output/
│   └── MegaSena/              # Generated CSV files
└── lottery-analytics.sln
```

### Getting Started

#### Prerequisites

- .NET 6.0 SDK or later

#### Running the Project

```bash
# Build the project
dotnet build

# Run the analysis
dotnet run --project MegaSena
```

#### Output

The program generates:

- **60+ cycle CSV files** in `Output/MegaSena/` (one per completed cycle)
- **Current cycle file** (`Cycle_InProgress_Concurso_XXXX.csv`)
- **Console predictions** with recommended betting combinations

### Sample Output

```
Current Cycle Status:
Numbers remaining: 2, 20, 43
Total remaining: 3

=== RECOMMENDED BETS SUMMARY ===
Top 3 recommended complete bets:

Bet #1: 7-12-35-39-43-56
  Strategy: Mid-range frequency (2-4 times) with remaining number 43

Bet #2: 17-19-20-22-23-55
  Strategy: Mid-range frequency (2-4 times) with remaining number 20

Bet #3: 2-12-15-39-46-47
  Strategy: Mid-range frequency (2-4 times) with remaining number 2
```

### Important Note

⚠️ **Educational Purpose**: This project is designed for statistical analysis and educational purposes. Each lottery draw is mathematically independent with no predictive power. Historical patterns provide interesting insights about lottery behavior over time but do not guarantee future results.

### Future Expansion

The project structure is designed to support additional Brazilian lottery games:

- Quina
- Lotofácil
- Lotomania
- And more...

Each game will have its own folder under `Output/` for organized data management.

## Contributing

Contributions are welcome! Feel free to submit issues or pull requests.

## License

This project is open source and available for educational and analytical purposes.
