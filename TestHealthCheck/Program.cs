using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TestHealthCheck;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Manual 

//builder.Services.AddHealthChecks();

//builder.Services.AddHealthChecksUI(x =>
//{
//    x.SetEvaluationTimeInSeconds(30);
//    x.MaximumHistoryEntriesPerEndpoint(100);
//    x.SetApiMaxActiveRequests(1);

//    x.AddHealthCheckEndpoint("Health Check", "/health");
//}).AddInMemoryStorage();

builder.Services.AddHealthCheck();
    //.WithService("", "test", tags: new[] { "Service" })
    //.WithRabbitMq("guest", "guest", "localhost", tags: new[] { "Rabbit" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// adding the middleware , host health check ui on `health-ui` endpoint by default

app.UseHealthCheck();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
