using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces
{
    public interface IVehiclesService
    {
        public List<Vehicle> GetVehicles(int? PageNumber = 1, string? Name = null, string? Brand = null);
        public Vehicle? GetVehicle(int Id);
        Vehicle Create(Vehicle Vehicle);
        void Update(Vehicle Vehicle);
        public void Delete(Vehicle Vehicle);
    }
}