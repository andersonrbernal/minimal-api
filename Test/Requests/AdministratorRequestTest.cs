using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Bogus;
using FluentAssertions;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.DTOs.Enums;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.ModelViews;
using Test.Domain.Helpers;

namespace Test.Requests;

[TestClass]
[DoNotParallelize]
public sealed class AdministratorRequestTest
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
            endpoint: "/administrators/login",
            loginDTO: LoginDTO,
            mediaType: MediaTypeJsonApplication,
            options: GetJsonOptions()
        );
    }

    [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
    public static void ClassCleanup() => Setup.ClassCleanup();

    [TestMethod("Login")]
    public async Task AdministratorLoginTest()
    {
        string strinfiedLoginDTO = JsonSerializer.Serialize(LoginDTO);
        var content = new StringContent(strinfiedLoginDTO, Encoding.UTF8, MediaTypeJsonApplication);

        var response = await Setup.client.PostAsync("/administrators/login", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadAsStringAsync();
        var loggedAdministrator = JsonSerializer.Deserialize<SignedInAdministratorModelView>(result, GetJsonOptions());

        loggedAdministrator.Should().NotBeNull();
        loggedAdministrator.Email.Should().Be(LoginDTO.Email);
        jwtHandler.CanReadToken(loggedAdministrator.Token).Should().BeTrue();

        JwtToken = loggedAdministrator.Token;
    }

    [TestMethod("GetAdmin")]
    public async Task GetAdministratorTest()
    {
        Administrator administrator = new Faker<Administrator>()
            .RuleFor(administrator => administrator.Email, faker => faker.Internet.Email())
            .RuleFor(administrator => administrator.Password, faker => faker.Internet.Password())
            .RuleFor(administrator => administrator.Profile, faker => faker.PickRandom<Profile>())
            .Generate();

        var stringfiedAdmin = JsonSerializer.Serialize(administrator);

        var request = new HttpRequestMessage(HttpMethod.Post, "/administrators");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        request.Content = new StringContent(stringfiedAdmin, Encoding.UTF8, MediaTypeJsonApplication);

        var response = await Setup.client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadAsStringAsync();
        var createdAdministrator = JsonSerializer.Deserialize<Administrator>(result, GetJsonOptions());

        createdAdministrator.Should().NotBeNull();
        createdAdministrator.Id.Should().BeGreaterThan(0);
        createdAdministrator.Email.Should().Be(administrator.Email);
        createdAdministrator.Password.Should().Be(administrator.Password);
        createdAdministrator.Profile.Should().Be(administrator.Profile);

        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/administrators/{createdAdministrator.Id}");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        var getResponse = await Setup.client.SendAsync(getRequest);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResult = await getResponse.Content.ReadAsStringAsync();
        var getAdministrator = JsonSerializer.Deserialize<Administrator>(getResult, GetJsonOptions());

        getAdministrator.Should().NotBeNull();
        getAdministrator.Id.Should().Be(createdAdministrator.Id);
        getAdministrator.Email.Should().Be(createdAdministrator.Email);
        getAdministrator.Password.Should().Be(createdAdministrator.Password);
        getAdministrator.Profile.Should().Be(createdAdministrator.Profile);
    }

    [TestMethod("GetAdmins")]
    public async Task GetAdministratorsTest()
    {
        Administrator administrator1 = new Faker<Administrator>()
            .RuleFor(administrator => administrator.Email, faker => faker.Internet.Email())
            .RuleFor(administrator => administrator.Password, faker => faker.Internet.Password())
            .RuleFor(administrator => administrator.Profile, faker => faker.PickRandom<Profile>())
            .Generate();

        var stringfiedAdmin1 = JsonSerializer.Serialize(administrator1);

        var request1 = new HttpRequestMessage(HttpMethod.Post, "/administrators");
        request1.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        request1.Content = new StringContent(stringfiedAdmin1, Encoding.UTF8, MediaTypeJsonApplication);

        var response1 = await Setup.client.SendAsync(request1);

        response1.StatusCode.Should().Be(HttpStatusCode.Created);

        var result1 = await response1.Content.ReadAsStringAsync();
        var createdAdministrator1 = JsonSerializer.Deserialize<Administrator>(result1, GetJsonOptions());

        createdAdministrator1.Should().NotBeNull();
        createdAdministrator1.Id.Should().BeGreaterThan(0);
        createdAdministrator1.Email.Should().Be(administrator1.Email);
        createdAdministrator1.Password.Should().Be(administrator1.Password);
        createdAdministrator1.Profile.Should().Be(administrator1.Profile);

        var getRequest1 = new HttpRequestMessage(HttpMethod.Get, $"/administrators/{createdAdministrator1.Id}");
        getRequest1.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        var getResponse1 = await Setup.client.SendAsync(getRequest1);

        getResponse1.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResult1 = await getResponse1.Content.ReadAsStringAsync();
        var getAdministrator1 = JsonSerializer.Deserialize<Administrator>(getResult1, GetJsonOptions());

        getAdministrator1.Should().NotBeNull();
        getAdministrator1.Id.Should().Be(createdAdministrator1.Id);
        getAdministrator1.Email.Should().Be(createdAdministrator1.Email);
        getAdministrator1.Password.Should().Be(createdAdministrator1.Password);
        getAdministrator1.Profile.Should().Be(createdAdministrator1.Profile);

        //////////////////////////////////////////////////////////////////////

        Administrator administrator2 = new Faker<Administrator>()
            .RuleFor(administrator => administrator.Email, faker => faker.Internet.Email())
            .RuleFor(administrator => administrator.Password, faker => faker.Internet.Password())
            .RuleFor(administrator => administrator.Profile, faker => faker.PickRandom<Profile>())
            .Generate();

        var stringfiedAdmin2 = JsonSerializer.Serialize(administrator2);

        var request2 = new HttpRequestMessage(HttpMethod.Post, "/administrators");
        request2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        request2.Content = new StringContent(stringfiedAdmin2, Encoding.UTF8, MediaTypeJsonApplication);

        var response2 = await Setup.client.SendAsync(request2);

        response2.StatusCode.Should().Be(HttpStatusCode.Created);

        var result2 = await response2.Content.ReadAsStringAsync();
        var createdAdministrator2 = JsonSerializer.Deserialize<Administrator>(result2, GetJsonOptions());
        Setup.TestContext.WriteLine(result2);

        createdAdministrator2.Should().NotBeNull();
        createdAdministrator2.Id.Should().BeGreaterThan(0);
        createdAdministrator2.Email.Should().Be(administrator2.Email);
        createdAdministrator2.Password.Should().Be(administrator2.Password);
        createdAdministrator2.Profile.Should().Be(administrator2.Profile);

        var getRequest2 = new HttpRequestMessage(HttpMethod.Get, $"/administrators/{createdAdministrator2.Id}");
        getRequest2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        var getResponse2 = await Setup.client.SendAsync(getRequest2);

        getResponse2.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResult2 = await getResponse2.Content.ReadAsStringAsync();
        var getAdministrator2 = JsonSerializer.Deserialize<Administrator>(getResult2, GetJsonOptions());
        Setup.TestContext.WriteLine(getResult2);

        getAdministrator2.Should().NotBeNull();
        getAdministrator2.Id.Should().Be(createdAdministrator2.Id);
        getAdministrator2.Email.Should().Be(createdAdministrator2.Email);
        getAdministrator2.Password.Should().Be(createdAdministrator2.Password);
        getAdministrator2.Profile.Should().Be(createdAdministrator2.Profile);
    }

    [TestMethod("CreateAdmin")]
    public async Task CreateAdministratorTest()
    {
        Administrator administrator = new Faker<Administrator>()
            .RuleFor(administrator => administrator.Email, faker => faker.Internet.Email())
            .RuleFor(administrator => administrator.Password, faker => faker.Internet.Password())
            .RuleFor(administrator => administrator.Profile, faker => faker.PickRandom<Profile>())
            .Generate();

        var stringfiedAdministrator = JsonSerializer.Serialize(administrator);

        var request = new HttpRequestMessage(HttpMethod.Post, "/administrators");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        request.Content = new StringContent(stringfiedAdministrator, Encoding.UTF8, MediaTypeJsonApplication);

        var response = await Setup.client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadAsStringAsync();
        var createdAdministrator = JsonSerializer.Deserialize<Administrator>(result, GetJsonOptions());

        createdAdministrator.Should().NotBeNull();
        createdAdministrator.Id.Should().BeGreaterThan(0);
        createdAdministrator.Email.Should().Be(administrator.Email);
        createdAdministrator.Password.Should().Be(administrator.Password);
        createdAdministrator.Profile.Should().Be(administrator.Profile);
    }

    [TestMethod("UpdateAdministrator")]
    public async Task UpdateAdministratorTest()
    {
        Administrator administrator = new Faker<Administrator>()
            .RuleFor(administrator => administrator.Email, faker => faker.Internet.Email())
            .RuleFor(administrator => administrator.Password, faker => faker.Internet.Password())
            .RuleFor(administrator => administrator.Profile, faker => faker.PickRandom<Profile>())
            .Generate();

        var stringfiedAdmin = JsonSerializer.Serialize(administrator);
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/administrators");
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        createRequest.Content = new StringContent(stringfiedAdmin, Encoding.UTF8, MediaTypeJsonApplication);

        var createResponse = await Setup.client.SendAsync(createRequest);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createResult = await createResponse.Content.ReadAsStringAsync();
        var createdAdministrator = JsonSerializer.Deserialize<Administrator>(createResult, GetJsonOptions());

        createdAdministrator.Should().NotBeNull();
        createdAdministrator.Id.Should().BeGreaterThan(0);
        createdAdministrator.Email.Should().Be(administrator.Email);
        createdAdministrator.Password.Should().Be(administrator.Password);
        createdAdministrator.Profile.Should().Be(administrator.Profile);

        Administrator updatingAdministrator = new Faker<Administrator>()
            .RuleFor(administrator => administrator.Email, faker => faker.Internet.ExampleEmail())
            .RuleFor(administrator => administrator.Password, faker => faker.Internet.Password())
            .RuleFor(administrator => administrator.Profile, faker => faker.PickRandomWithout(createdAdministrator.Profile))
            .Generate();

        var stringfiedUpdatingAdmin = JsonSerializer.Serialize(updatingAdministrator);
        var updateRequest = new HttpRequestMessage(HttpMethod.Patch, $"/administrators/{createdAdministrator.Id}");
        updateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        updateRequest.Content = new StringContent(stringfiedUpdatingAdmin, Encoding.UTF8, MediaTypeJsonApplication);

        var updateResponse = await Setup.client.SendAsync(updateRequest);
        var updateResult = await updateResponse.Content.ReadAsStringAsync();
        var updatedAdministrator = JsonSerializer.Deserialize<Administrator>(updateResult, GetJsonOptions());

        updatedAdministrator.Should().NotBeNull();
        updatedAdministrator.Id.Should().Be(createdAdministrator.Id);
        updatedAdministrator.Email.Should().NotBe(createdAdministrator.Email);
        updatedAdministrator.Password.Should().NotBe(createdAdministrator.Password);
        updatedAdministrator.Profile.Should().NotBe(createdAdministrator.Profile);
    }

    [TestMethod("DeleteAdministrator")]
    public async Task DeleteAdministratorTest()
    {
        var administrator = new Faker<Administrator>()
            .RuleFor(admin => admin.Email, faker => faker.Internet.Email())
            .RuleFor(admin => admin.Password, faker => faker.Internet.Password())
            .RuleFor(admin => admin.Profile, faker => faker.PickRandom<Profile>())
            .Generate();

        var stringfiedAdmin = JsonSerializer.Serialize(administrator);
        var request = new HttpRequestMessage(HttpMethod.Post, "/administrators");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        request.Content = new StringContent(stringfiedAdmin, Encoding.UTF8, MediaTypeJsonApplication);

        var response = await Setup.client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadAsStringAsync();
        var createdAdministrator = JsonSerializer.Deserialize<Administrator>(result, GetJsonOptions());

        createdAdministrator.Should().NotBeNull();
        createdAdministrator.Email.Should().Be(administrator.Email);
        createdAdministrator.Password.Should().Be(administrator.Password);
        createdAdministrator.Profile.Should().Be(administrator.Profile);

        var requestForDeletion = new HttpRequestMessage(HttpMethod.Delete, $"/administrators/{createdAdministrator.Id}");
        var stringfiedCreatedAdmin = JsonSerializer.Serialize(createdAdministrator);
        requestForDeletion.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        requestForDeletion.Content = new StringContent(stringfiedAdmin, Encoding.UTF8, MediaTypeJsonApplication);

        var responseForDeletion = await Setup.client.SendAsync(requestForDeletion);
        var resultForDeletion = await responseForDeletion.Content.ReadAsStringAsync();

        responseForDeletion.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}