var builder = WebApplication.CreateBuilder(args);



builder.Services.AddHealthChecksUI(i =>
{
    i.MaximumHistoryEntriesPerEndpoint(60);
    i.SetHeaderText("Health Check Management");
    i.DisableDatabaseMigrations();

}).AddInMemoryStorage();

var app = builder.Build();
app.UseRouting();

app.UseHealthChecksUI(setup =>
{
    setup.UIPath = "/ui";
});

app.UseEndpoints(endpoint => endpoint.MapGet("/", context =>
{
    return Task.Run(() => context.Response.Redirect("ui"));
}));

app.UseHttpsRedirection();

app.Run();
