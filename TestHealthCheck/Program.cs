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

builder.Services.AddHealthCheck(); // add following lines for external dependencies
                                   //.WithSqlServer("")
                                   //.WithRabbitMq("user", "pass", "server")
                                   //.WithRedis("con");

builder.Services.AddHealthCheckUi("test application"); // adding the health check api on `health` endpoint

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

app.UseHealthChecksUI();
app.Run();
