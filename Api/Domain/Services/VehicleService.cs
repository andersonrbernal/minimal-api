using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastructure.Database;

namespace MinimalApi.Domain.Services
{
    public class VehiclesService(DatabaseContext context) : IVehiclesService
    {
        private readonly DatabaseContext _context = context;

        public Vehicle Create(Vehicle Vehicle)
        {
            _context.Vehicles.Add(Vehicle);
            _context.SaveChanges();
            return Vehicle;
        }

        public void Delete(Vehicle Vehicle)
        {
            _context.Vehicles.Remove(Vehicle);
            _context.SaveChanges();
        }

        public Vehicle? GetVehicle(int Id)
        {
            return _context.Vehicles.Where(vehicle => vehicle.Id == Id).FirstOrDefault();
        }

        public List<Vehicle> GetVehicles(int? PageNumber = 1, string? Name = null, string? Brand = null)
        {
            IQueryable<Vehicle> query = _context.Vehicles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(Name))
                query = query.Where(vehicle => EF.Functions.Like(vehicle.Name.ToLower(), $"%{Name.ToLower()}%"));

            if (!string.IsNullOrWhiteSpace(Brand))
                query = query.Where(vehicle => EF.Functions.Like(vehicle.Brand.ToLower(), $"%{Brand.ToLower()}%"));


            if (PageNumber != null)
            {
                int vehiclesPerPage = 10;
                int skip = ((int)PageNumber - 1) * vehiclesPerPage; // skips some registries to return the vehicles per page 
                query = query.Skip(skip).Take(vehiclesPerPage);
            }

            return [.. query];
        }

        public void Update(Vehicle Vehicle)
        {
            _context.Vehicles.Update(Vehicle);
            _context.SaveChanges();
        }
    }
}
