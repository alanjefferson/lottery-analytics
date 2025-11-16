using MegaSena;
using MegaSena.Core;
using MegaSena.Entity;


List<MegaSenaDraw> lstMegaSena = MegaSenaResults.ReadMegaSenaXLSX();

CycleResults objCycleResults = new CycleResults();
List<Cycle> lstCycle = objCycleResults.GetCycleList(lstMegaSena);

// Run analysis

//AnalyzeOneTimeNumbers.Analyze();

//AnalyzeCycleDuration.Analyze();

//AnalyzeThreeNumbersRemaining.Analyze(lstMegaSena);

//AnalyzeDrawsUntilRemainingPicked.Analyze(lstMegaSena);

//AnalyzeFirstRemainingNumber.Analyze(lstMegaSena);

//AnalyzeRemainingNumbers.Analyze(lstMegaSena, 3);

// Generate predictions for current cycle (whatever the remaining count is)
PredictNextDraw.GeneratePredictions(lstMegaSena);








