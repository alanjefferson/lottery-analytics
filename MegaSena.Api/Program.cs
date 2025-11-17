using MegaSena.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddScoped<PredictionService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MegaSena Prediction API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

app.UseHttpsRedirection();

// MegaSena Prediction Endpoint
app.MapGet("/api/predictions/next-draw", (PredictionService predictionService) =>
{
    try
    {
        var prediction = predictionService.GetNextDrawPrediction();
        return Results.Ok(prediction);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Error generating prediction"
        );
    }
})
.WithName("GetNextDrawPrediction")
.WithTags("Predictions")
.Produces<MegaSena.Api.Models.PredictionResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

// Health check endpoint
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.Run();
