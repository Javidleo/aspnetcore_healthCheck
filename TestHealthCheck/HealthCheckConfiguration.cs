using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace TestHealthCheck;

public static class HealthCheckConfiguration
{
    public static IHealthChecksBuilder AddHealthCheck(this IServiceCollection services)
    {
        return services.AddHealthChecks();
    }

    public static IHealthChecksBuilder WithSqlServer(this IHealthChecksBuilder builder,
        string connectionString,
        HealthStatus failureStatus = HealthStatus.Unhealthy,
        string name = "Sql Server",
        IEnumerable<string>? tags = null
        )
    {
        builder.AddSqlServer(connectionString,
            failureStatus: failureStatus,
            name: name,
            tags: tags);

        return builder;
    }

    public static IHealthChecksBuilder WithRabbitMq(this IHealthChecksBuilder builder,
        string username,
        string password,
        string server,
        int port = 5672,
        HealthStatus failureStatus = HealthStatus.Unhealthy,
        SslOption? sslOption = default,
        string name = "RabbitMq",
        TimeSpan? timeout = default,
        IEnumerable<string>? tags = null)
    {
        var connection = $"amqp://{username}:{password}@{server}:{port}";

        builder.AddRabbitMQ(new Uri(connection),
            sslOption: sslOption,
            name: name,
            failureStatus: failureStatus,
            timeout: timeout,
            tags: tags);

        return builder;
    }

    public static IHealthChecksBuilder WithRedis(this IHealthChecksBuilder builder,
        string redisConnectionString,
        string name = "Redis",
        HealthStatus? failureStatus = HealthStatus.Unhealthy,
        TimeSpan? timeout = default,
        IEnumerable<string>? tags = null
        )
    {
        return builder.AddRedis(_ =>
            redisConnectionString,
            name,
            failureStatus,
            tags,
            timeout
            );
    }

    /// <summary>
    /// adds a custom ui for your application,
    /// put this method after health check service
    /// </summary>
    /// <param name="services"></param>
    /// <param name="name"> application name</param>
    /// <param name="apiEndPoint"></param>
    /// <param name="evaluationTime">time between each duration</param>
    /// <param name="maximumHistory">maximum cached health checks</param>
    /// <param name="maximumActiveRequests"> maximum concurrent requests</param>
    /// <returns></returns>
    public static IServiceCollection AddHealthCheckUi(this IServiceCollection services,
        string name,
        string apiEndPoint = "health",
        int evaluationTime = 60,
        int maximumHistory = 60,
        int maximumActiveRequests = 1
        )
    {
        services.AddHealthChecksUI(opt =>
        {
            opt.SetEvaluationTimeInSeconds(evaluationTime); //time in seconds between each check
            opt.MaximumHistoryEntriesPerEndpoint(maximumHistory); //maximum history of checks
            opt.SetApiMaxActiveRequests(maximumActiveRequests); //api requests concurrency

            opt.AddHealthCheckEndpoint(name, $"/{apiEndPoint}");
        })
        .AddInMemoryStorage();

        return services;
    }

    public static IHealthChecksBuilder WithService(this IHealthChecksBuilder builder,
        string uri,
        string name,
        string healthCheckEndpoint = "health",
        HealthStatus? failureStatus = HealthStatus.Unhealthy,
        TimeSpan? timeout = null,
        IEnumerable<string>? tags = null)
    {
        timeout ??= TimeSpan.FromSeconds(30);

        builder.AddUrlGroup(new Uri($"{uri}/{healthCheckEndpoint}"),
            name,
            failureStatus,
            timeout: timeout,
            tags: tags
        );

        return builder;
    }



    /// <summary>
    /// specifies the http status code for health status,
    /// <para> attention : put this method after app.UseRouting() method.</para> 
    /// <para>Healthy returns 200OK. </para> 
    /// <para>Degraded returns 500InternalServerError.</para> 
    /// <para>Unhealthy returns 503ServerUnavailable.</para>
    /// <para>if you want to set rate limiting, add </para>
    /// </summary>
    /// <param name="app"></param>
    /// <param name="apiEndpoint"></param>
    /// <param name="uiEndpoint"></param>
    /// <returns></returns>
    public static WebApplication UseHealthCheck(this WebApplication app,
        string apiEndpoint = "health")
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks($"/{apiEndpoint}", new HealthCheckOptions()
            {
                ResultStatusCodes =
                {

                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecksUI();
        });
        return app;
    }

}
