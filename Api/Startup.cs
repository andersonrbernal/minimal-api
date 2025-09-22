using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.DTOs.Enums;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Extentions;
using MinimalApi.Domain.Filters;
using MinimalApi.Domain.FormValidation;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Services;
using MinimalApi.Infrastructure.Database;

namespace MinimalApi;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; set; } = configuration;
    private string JwtKey { get; set; } = configuration.GetSection("Jwt:Key").Value ?? "";
    public void ConfigureServices(IServiceCollection services)
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => options.TokenValidationParameters = tokenValidationParameters);

        services.AddAuthorization();

        services.AddScoped<IAdministratorService, AdministratorService>();
        services.AddScoped<IVehiclesService, VehiclesService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Please enter into field yourJWT token"
            });

            options.OperationFilter<AddAuthHeaderOperationFilter>();
        });

        services.AddDbContext<DatabaseContext>(options =>
        {
            var connectionString = Configuration.GetConnectionString("Mysql");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoint =>
        {
            var roles = string.Join(",", [
                Profile.ADMINISTRATOR.GetDescription(),
                Profile.EDITOR.GetDescription()
            ]);

            #region Home
            endpoint.MapGet("/", () => Results.Json(new ValidationResultInfo()))
               .AllowAnonymous()
               .WithTags("Home");
            #endregion

            #region Administrators
            string GenerateTokenJwt(Administrator administrator)
            {
                SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(JwtKey));
                SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
                List<Claim> claims =
                [
                    new Claim(nameof(administrator.Email), administrator.Email),
                    new Claim(ClaimTypes.Role, administrator.Profile.GetDescription())
                ];
                JwtSecurityToken token = new(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                JwtSecurityTokenHandler tokenHandler = new();
                return tokenHandler.WriteToken(token);
            }

            endpoint.MapPost("/administrators/login", ([FromBody] LoginDTO LoginDTO, IAdministratorService administratorService) =>
            {
                try
                {
                    Administrator? administrator = administratorService.Login(LoginDTO);
                    if (administrator == null) return Results.Unauthorized();

                    string token = GenerateTokenJwt(administrator: administrator);
                    SignedInAdministratorModelView signedInAdministrator = new()
                    {
                        Email = administrator.Email,
                        Token = token,
                        Id = administrator.Id
                    };

                    return Results.Ok(signedInAdministrator);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            }).AllowAnonymous().WithTags("Administrators");

            endpoint.MapPost("/administrators", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
            {
                try
                {
                    Administrator administrator = new();
                    administratorDTO.Adapt(administrator);

                    ValidationErrorModelView validation = AdministratorValidation.Validate(administratorDTO);
                    if (validation.Messages.Count > 0)
                        return Results.BadRequest(validation);

                    administratorService.Create(administrator);
                    return Results.Created($"/administrators/{administrator.Id}", administrator);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = Profile.ADMINISTRATOR.GetDescription() })
            .WithTags("Administrators");

            endpoint.MapPatch("/administrators/{id}", ([FromRoute] int id, [FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
            {
                try
                {
                    Administrator? administrator = administratorService.GetAdministrator(id);
                    if (administrator == null) return Results.NotFound();

                    administratorDTO.Adapt(administrator);
                    administratorService.Update(administrator);

                    return Results.Ok(administrator);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = Profile.ADMINISTRATOR.GetDescription() })
            .WithTags("Administrators");

            endpoint.MapGet("/administrators", ([FromQuery] int? PageNumber, IAdministratorService administratorService) =>
            {
                try
                {
                    return Results.Ok(administratorService.GetAdministrators(PageNumber));
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = Profile.ADMINISTRATOR.GetDescription() })
            .WithTags("Administrators");

            endpoint.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
            {
                try
                {
                    Administrator? administrator = administratorService.GetAdministrator(id);
                    if (administrator == null) return Results.NotFound();

                    return Results.Ok(administrator);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = Profile.ADMINISTRATOR.GetDescription() })
            .WithTags("Administrators");

            endpoint.MapDelete("/administrators/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
            {
                try
                {
                    Administrator? administrator = administratorService.GetAdministrator(id);
                    if (administrator == null) return Results.NotFound();

                    administratorService.Delete(administrator);
                    return Results.Ok(administrator);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = Profile.ADMINISTRATOR.GetDescription() })
            .WithTags("Administrators");
            #endregion

            #region Vehicles
            endpoint.MapGet("/vehicles/", ([FromQuery] int? pageNumber, IVehiclesService vehiclesService) =>
            {
                try
                {
                    List<Vehicle> vehicles = vehiclesService.GetVehicles(pageNumber);
                    return Results.Ok(vehicles);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = roles })
            .WithTags("Vehicles");

            endpoint.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehiclesService vehiclesService) =>
            {
                try
                {
                    Vehicle? vehicle = vehiclesService.GetVehicle(id);
                    if (vehicle == null) return Results.NotFound();

                    return Results.Ok(vehicle);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = roles })
            .WithTags("Vehicles");

            endpoint.MapPost("/vehicles/", ([FromBody] VehicleDTO vehicleDTO, IVehiclesService vehiclesService) =>
            {
                try
                {
                    ValidationErrorModelView validation = VehicleValidation.Validate(vehicleDTO);
                    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

                    Vehicle vehicle = vehicleDTO.Adapt(new Vehicle());
                    vehiclesService.Create(vehicle);

                    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = roles })
            .WithTags("Vehicles");

            endpoint.MapPatch("/vehicles/{id}", ([FromRoute] int id, [FromBody] VehicleDTO vehicleDTO, IVehiclesService vehiclesService) =>
            {
                try
                {
                    Vehicle? vehicle = vehiclesService.GetVehicle(id);
                    if (vehicle == null) return Results.NotFound();

                    vehicleDTO.Adapt(vehicle);
                    vehiclesService.Update(vehicle);

                    return Results.Ok(vehicle);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = Profile.ADMINISTRATOR.GetDescription() })
            .WithTags("Vehicles");

            endpoint.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehiclesService vehiclesService) =>
            {
                try
                {
                    Vehicle? vehicle = vehiclesService.GetVehicle(id);
                    if (vehicle == null) return Results.NotFound();

                    vehiclesService.Delete(vehicle);
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = Profile.ADMINISTRATOR.GetDescription() })
            .WithTags("Vehicles");
            #endregion
        });
    }
}