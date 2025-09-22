using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? GetAdministrator(int Id);
        List<Administrator> GetAdministrators(int? PageNumber = 1);
        Administrator Create(Administrator administrator);
        void Update(Administrator administrator);
        void Delete(Administrator administrator);
        public Administrator? Login(LoginDTO loginDTO);
    }
}