
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi;
using MinimalApi.Domain.Interfaces;
using Test.Mocks.Services;

namespace Test.Domain.Helpers;

public class Setup
{
    public const string PORT = "5001";
    public static TestContext TestContext = default!;
    public static WebApplicationFactory<Startup> http = default!;
    public static HttpClient client = default!;

    public static void ClassInit(TestContext testContext)
    {
        TestContext = testContext;
        http = new WebApplicationFactory<Startup>();
        http = http.WithWebHostBuilder(builder => InitBuilder(builder));
        client = http.CreateClient();
    }

    public static void ClassCleanup()
    {
        http.Dispose();
    }

    public static void TestInitialize()
    {
        //
    }

    private static IWebHostBuilder InitBuilder(IWebHostBuilder builder)
    {
        builder.UseSetting("http_port", PORT).UseEnvironment("Testing");
        builder.ConfigureServices(services => InitServices(services));
        return builder;
    }

    private static IServiceCollection InitServices(IServiceCollection services)
    {
        services.AddScoped<IAdministratorService, AdministratorServiceMock>();
        services.AddScoped<IVehiclesService, VehicleServiceMock>();
        return services;
    }
}