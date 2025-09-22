using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Services;
using MinimalApi.Infrastructure.Database;

namespace Test.Domain.Services;

[TestClass]
[DoNotParallelize]
public sealed class VehicleServiceTest
{
    public TestContext TestContext { get; set; }
    private DatabaseContext _context = default!;
    private VehiclesService _service = default!;

    private void ResetVehicles(DatabaseContext context)
    {
        context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;");
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE vehicles;");
        context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 1;");
    }

    private static DatabaseContext InitDatabaseContextTest()
    {
        var basePath = AppContext.BaseDirectory;
        var jsonPath = Path.Combine(basePath, "appsettings.json");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseMySql(configuration.GetConnectionString("Mysql"), ServerVersion.AutoDetect(configuration.GetConnectionString("Mysql")))
            .Options;

        return new DatabaseContext(options, configuration);
    }

    [TestInitialize]
    public void Setup()
    {
        _context = InitDatabaseContextTest();
        _service = new(_context);
        ResetVehicles(_context);
    }

    [TestCleanup]
    public void Teardown()
    {
        _context.Dispose();
    }

    [TestMethod("GetVehicles")]
    public void TestGetVehicles()
    {
        List<Vehicle> vehicles =
        [
            new() { Name = "Fiat",  Brand = "Siena", Year = 2011 },
            new() { Name = "Fusca", Brand = "VW",    Year = 1978 }
        ];

        _service.Create(vehicles[0]);
        _service.Create(vehicles[1]);

        var foundVehicles = _service.GetVehicles();
        foundVehicles.Should().BeEquivalentTo(
            vehicles,
            options => options
                .WithoutStrictOrdering()
                .Excluding(vehicle => vehicle.Id)
        );
    }

    [TestMethod("GetVehicle")]
    public void TestGetVehicle()
    {
        Vehicle vehicle = new() { Name = "Siena", Brand = "Fiat", Year = 2011 };
        _service.Create(vehicle);

        var foundVehicle = _service.GetVehicle(vehicle.Id);

        Assert.AreEqual(vehicle, foundVehicle);
    }

    [TestMethod("UpdateVehicle")]
    public void TestUpdateVehicle()
    {
        Vehicle vehicle = new() { Name = "Siena", Brand = "Fiat", Year = 2011 };
        var updatedVehicle = vehicle.Adapt<Vehicle>();
        updatedVehicle.Name = "Siena El Flex";

        _service.Create(vehicle);
        _service.Update(updatedVehicle);

        Assert.AreNotEqual(vehicle.Name, updatedVehicle.Name);
    }

    [TestMethod("DeleteVehicle")]
    public void TestDeleteVehicle()
    {
        Vehicle vehicle = new() { Name = "Siena", Brand = "Fiat", Year = 2011 };

        _service.Create(vehicle);
        _service.Delete(vehicle);

        var foundVehicle = _service.GetVehicle(vehicle.Id);

        Assert.IsNull(foundVehicle);
        Assert.AreNotEqual(vehicle, foundVehicle);
    }
}