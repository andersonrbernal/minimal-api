using System.Reflection;
using System.Text.Json;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Domain.DTOs.Enums;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Services;
using MinimalApi.Infrastructure.Database;

namespace Test.Domain.Services;

[TestClass]
[DoNotParallelize]
public sealed class AdministratorServiceTest
{
    public TestContext TestContext { get; set; }

    private DatabaseContext _context = default!;
    private AdministratorService _service = default!;

    private void ResetAdministrators(DatabaseContext context)
    {
        context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;");
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administrators;");
        context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 1;");
    }

    [TestInitialize]
    public void Setup()
    {
        _context = InitDatabaseContextTest();
        _service = new(_context);
        ResetAdministrators(_context);
    }

    [TestCleanup]
    public void Teardown()
    {
        _context.Dispose();
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

    [TestMethod("FindAdministratorByIdAfterCreation")]
    public void TestFindAdministratorByIdAfterCreation()
    {
        Administrator administrator = new()
        {
            Email = "administrator@minimal_api.com",
            Password = "12345678",
            Profile = Profile.ADMINISTRATOR
        };

        _service.Create(administrator);

        var foundAdministrator = _service.GetAdministrator(administrator.Id);

        Assert.IsNotNull(foundAdministrator);
        Assert.IsTrue(administrator.Id > 0);
        Assert.AreEqual(administrator.Id, foundAdministrator!.Id);
    }

    [TestMethod("FindAdministratorsByIdAfterCreation")]
    public void TestFindAdministratorsByIdAfterCreation()
    {
        List<Administrator> administrators = [
            new() {
                Email = "administrator@minimal_api.com",
                Password = "12346578",
                Profile = Profile.ADMINISTRATOR
            },
            new() {
                Email = "editor@minimal_api.com",
                Password = "password",
                Profile = Profile.EDITOR
            }
        ];

        _service.Create(administrators[0]);
        _service.Create(administrators[1]);

        var foundAdministrators = _service.GetAdministrators();
        var foundAdministrator = foundAdministrators.Find(admin => admin.Id == administrators[0].Id);
        var foundEditor = foundAdministrators.Find(admin => admin.Id == administrators[1].Id);

        Assert.IsNotNull(foundAdministrator);
        Assert.IsNotNull(foundEditor);

        Assert.AreEqual(administrators[0].Id, foundAdministrator.Id);
        Assert.AreEqual(administrators[0].Email, foundAdministrator.Email);
        Assert.AreEqual(administrators[0].Password, foundAdministrator.Password);
        Assert.AreEqual(administrators[0].Profile, foundAdministrator.Profile);

        Assert.AreEqual(administrators[1].Id, foundEditor.Id);
        Assert.AreEqual(administrators[1].Email, foundEditor.Email);
        Assert.AreEqual(administrators[1].Password, foundEditor.Password);
        Assert.AreEqual(administrators[1].Profile, foundEditor.Profile);
    }

    [TestMethod("CreateAdministrator")]
    public void TestCreateAdministrator()
    {
        Administrator administrator = new()
        {
            Email = "administrator@minimal_api.com",
            Password = "12345678",
            Profile = Profile.ADMINISTRATOR
        };

        _service.Create(administrator);

        Assert.AreEqual(1, _service.GetAdministrators(1).Count);
    }

    [TestMethod("UpdateAdministrator")]
    public void TestUpdateAdministrator()
    {
        Administrator administrator = new()
        {
            Email = "administrator@minimal_api.com",
            Password = "12346578",
            Profile = Profile.ADMINISTRATOR
        };

        _service.Create(administrator);

        var originalProfile = administrator.Profile;

        administrator.Profile = Profile.EDITOR;

        _service.Update(administrator);

        var updatedAdministrator = _service.GetAdministrator(administrator.Id);

        Assert.IsNotNull(updatedAdministrator);
        Assert.AreEqual(Profile.EDITOR, updatedAdministrator.Profile);
        Assert.AreNotEqual(originalProfile, administrator.Profile);
    }

    [TestMethod("DeleteAdministrator")]
    public void TestDeleteAdministrator()
    {
        Administrator administrator = new()
        {
            Email = "administrator@minimal_api.com",
            Password = "12346578",
            Profile = Profile.ADMINISTRATOR
        };

        _service.Create(administrator);

        var createdAdministrator = _service.GetAdministrator(administrator.Id);

        _service.Delete(administrator);

        var deletedAdministrator = _service.GetAdministrator(administrator.Id);

        Assert.IsNull(deletedAdministrator);
        Assert.AreNotEqual(createdAdministrator, deletedAdministrator);
    }
}