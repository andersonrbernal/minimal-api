using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Bogus;
using FluentAssertions;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using Test.Domain.Helpers;

[TestClass]
[DoNotParallelize]
public sealed class VehicleRequestTest
{
    private readonly static JwtSecurityTokenHandler jwtHandler = new();
    private const string MediaTypeJsonApplication = "application/json";
    private static JsonSerializerOptions GetJsonOptions() => new() { PropertyNameCaseInsensitive = true };
    private static readonly LoginDTO LoginDTO = new() { Email = "administrator@minimal_api.com", Password = "12345678" };
    private static string? JwtToken { get; set; }

    [ClassInitialize]
    public static async Task ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
        JwtToken = await JwtAuthenticationHelper.GetJwtToken(
            "/administrators/login",
            LoginDTO,
            MediaTypeJsonApplication,
            GetJsonOptions()
        );
    }

    [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
    public static void ClassCleanup() => Setup.ClassCleanup();

    [TestMethod("GetVehicle")]
    public async Task GetVehicleTest()
    {
        var vehicle = new Faker<Vehicle>()
            .RuleFor(v => v.Name, f => f.Vehicle.Model())
            .RuleFor(v => v.Brand, f => f.Vehicle.Manufacturer())
            .RuleFor(v => v.Year, f => f.Random.Int(1900, DateTime.Now.Year))
            .Generate();

        string stringfiedVehicle = JsonSerializer.Serialize(vehicle, GetJsonOptions());

        var request = new HttpRequestMessage(HttpMethod.Post, "/vehicles");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        request.Content = new StringContent(stringfiedVehicle, Encoding.UTF8, MediaTypeJsonApplication);

        var response = await Setup.client.SendAsync(request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var result = await response.Content.ReadAsStringAsync();
        var createdVehicle = JsonSerializer.Deserialize<Vehicle>(result, GetJsonOptions());

        createdVehicle.Should().NotBeNull();
        createdVehicle.Id.Should().BeGreaterThan(0);
        createdVehicle.Name.Should().Be(vehicle.Name);
        createdVehicle.Brand.Should().Be(vehicle.Brand);
        createdVehicle.Year.Should().Be(vehicle.Year);

        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/vehicles/{createdVehicle.Id}");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);

        var getResponse = await Setup.client.SendAsync(getRequest);
        getResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var getResult = await getResponse.Content.ReadAsStringAsync();
        var foundVehicle = JsonSerializer.Deserialize<Vehicle>(getResult, GetJsonOptions());

        foundVehicle.Should().NotBeNull();
        foundVehicle.Id.Should().Be(createdVehicle.Id);
        foundVehicle.Name.Should().Be(createdVehicle.Name);
        foundVehicle.Brand.Should().Be(createdVehicle.Brand);
        foundVehicle.Year.Should().Be(createdVehicle.Year);
    }

    [TestMethod("CreateVehicle")]
    public async Task CreateVehicleTest()
    {
        var vehicle = new Faker<Vehicle>()
            .RuleFor(v => v.Name, f => f.Vehicle.Model())
            .RuleFor(v => v.Brand, f => f.Vehicle.Manufacturer())
            .RuleFor(v => v.Year, f => f.Random.Int(1900, DateTime.Now.Year))
            .Generate();

        string stringfiedVehicle = JsonSerializer.Serialize(vehicle, GetJsonOptions());

        var request = new HttpRequestMessage(HttpMethod.Post, "/vehicles");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        request.Content = new StringContent(stringfiedVehicle, Encoding.UTF8, MediaTypeJsonApplication);

        var response = await Setup.client.SendAsync(request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var result = await response.Content.ReadAsStringAsync();
        var createdVehicle = JsonSerializer.Deserialize<Vehicle>(result, GetJsonOptions());

        createdVehicle.Should().NotBeNull();
        createdVehicle.Id.Should().BeGreaterThan(0);
        createdVehicle.Name.Should().Be(vehicle.Name);
        createdVehicle.Brand.Should().Be(vehicle.Brand);
        createdVehicle.Year.Should().Be(vehicle.Year);
    }

    [TestMethod("UpdateVehicle")]
    public async Task UpdateVehicleTest()
    {
        var vehicle = new Faker<Vehicle>()
            .RuleFor(v => v.Name, f => f.Vehicle.Model())
            .RuleFor(v => v.Brand, f => f.Vehicle.Manufacturer())
            .RuleFor(v => v.Year, f => f.Random.Int(1900, DateTime.Now.Year))
            .Generate();

        string stringfiedVehicle = JsonSerializer.Serialize(vehicle, GetJsonOptions());

        var request = new HttpRequestMessage(HttpMethod.Post, "/vehicles");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        request.Content = new StringContent(stringfiedVehicle, Encoding.UTF8, MediaTypeJsonApplication);

        var response = await Setup.client.SendAsync(request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var result = await response.Content.ReadAsStringAsync();
        var createdVehicle = JsonSerializer.Deserialize<Vehicle>(result, GetJsonOptions());

        createdVehicle.Should().NotBeNull();
        createdVehicle.Id.Should().BeGreaterThan(0);
        createdVehicle.Name.Should().Be(vehicle.Name);
        createdVehicle.Brand.Should().Be(vehicle.Brand);
        createdVehicle.Year.Should().Be(vehicle.Year);

        var updatingVehicle = new Faker<Vehicle>()
            .RuleFor(v => v.Name, f => f.Vehicle.Model())
            .RuleFor(v => v.Brand, f => f.Vehicle.Manufacturer())
            .RuleFor(v => v.Year, f => f.Random.Int(1900, DateTime.Now.Year))
            .Generate();

        var stringfiedUpdatingVehicle = JsonSerializer.Serialize(updatingVehicle, GetJsonOptions());

        var updateRequest = new HttpRequestMessage(HttpMethod.Patch, $"/vehicles/{createdVehicle.Id}");
        updateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        updateRequest.Content = new StringContent(stringfiedUpdatingVehicle, Encoding.UTF8, MediaTypeJsonApplication);

        var updateResponse = await Setup.client.SendAsync(updateRequest);
        updateResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var updateResult = await updateResponse.Content.ReadAsStringAsync();
        var updatedVehicle = JsonSerializer.Deserialize<Vehicle>(updateResult, GetJsonOptions());

        updatedVehicle.Should().NotBeNull();
        updatedVehicle.Id.Should().Be(createdVehicle.Id);
        updatedVehicle.Name.Should().NotBe(createdVehicle.Name);
        updatedVehicle.Brand.Should().NotBe(createdVehicle.Brand);
        updatedVehicle.Year.Should().NotBe(createdVehicle.Year);
    }

    [TestMethod("DeleteVehicle")]
    public async Task DeleteVehicleTest()
    {
        var vehicle = new Faker<Vehicle>()
            .RuleFor(v => v.Name, f => f.Vehicle.Model())
            .RuleFor(v => v.Brand, f => f.Vehicle.Manufacturer())
            .RuleFor(v => v.Year, f => f.Random.Int(1900, DateTime.Now.Year))
            .Generate();

        string stringfiedVehicle = JsonSerializer.Serialize(vehicle, GetJsonOptions());

        var request = new HttpRequestMessage(HttpMethod.Post, "/vehicles");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        request.Content = new StringContent(stringfiedVehicle, Encoding.UTF8, MediaTypeJsonApplication);

        var response = await Setup.client.SendAsync(request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var result = await response.Content.ReadAsStringAsync();
        var createdVehicle = JsonSerializer.Deserialize<Vehicle>(result, GetJsonOptions());

        createdVehicle.Should().NotBeNull();
        createdVehicle.Id.Should().BeGreaterThan(0);
        createdVehicle.Name.Should().Be(vehicle.Name);
        createdVehicle.Brand.Should().Be(vehicle.Brand);
        createdVehicle.Year.Should().Be(vehicle.Year);

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/vehicles/{createdVehicle.Id}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);

        var deleteResponse = await Setup.client.SendAsync(deleteRequest);
        deleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }
}