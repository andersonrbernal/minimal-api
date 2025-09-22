using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;

namespace Test.Mocks.Services;

public class VehicleServiceMock : IVehiclesService
{
    private static List<Vehicle> vehicles { get; set; } = [
        new () {Id = 1, Name = "Siena", Brand = "Fiat", Year = 2011},
        new () {Id = 2, Name = "Fusca", Brand = "VW", Year = 1976}
    ];

    public Vehicle Create(Vehicle vehicle)
    {
        vehicle.Id = vehicles.Count + 1;
        vehicles.Add(vehicle);
        return vehicle;
    }

    public void Delete(Vehicle vehicle)
    {
        vehicles.Remove(vehicle);
    }

    public Vehicle? GetVehicle(int Id)
    {
        return vehicles.Where(v => v.Id == Id).FirstOrDefault();
    }

    public List<Vehicle> GetVehicles(int? PageNumber = 1, string? Name = null, string? Brand = null)
    {
        var query = vehicles.AsQueryable().AsNoTracking();

        if (string.IsNullOrWhiteSpace(Name))
            query = query.Where(v => v.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase));
        if (string.IsNullOrWhiteSpace(Brand))
            query = query.Where(v => v.Brand.Equals(Brand, StringComparison.CurrentCultureIgnoreCase));

        if (PageNumber != null)
        {
            int vehiclesPerPage = 10;
            int skip = ((int)PageNumber - 1) * vehiclesPerPage;
            query = query.Skip(skip).Take(vehiclesPerPage);
        }

        return [.. query];
    }

    public void Update(Vehicle vehicle)
    {
        vehicles
           .Where(v => v.Id == vehicle.Id)
           .FirstOrDefault()
           .Adapt(vehicle);
    }
}