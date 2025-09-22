using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastructure.Database;

namespace MinimalApi.Domain.Services
{
    public class AdministratorService(DatabaseContext context) : IAdministratorService
    {
        private readonly DatabaseContext _context = context;

        public Administrator Create(Administrator administrator)
        {
            _context.Administrators.Add(administrator);
            _context.SaveChanges();
            return administrator;
        }

        public void Delete(Administrator administrator)
        {
            _context.Administrators.Remove(administrator);
            _context.SaveChanges();
        }

        public Administrator? GetAdministrator(int Id)
        {
            return _context.Administrators.Where(administrator => administrator.Id == Id).FirstOrDefault();
        }

        public List<Administrator> GetAdministrators(int? PageNumber = 1)
        {
            IQueryable<Administrator> query = _context.Administrators.AsNoTracking();

            if (PageNumber != null)
            {
                int administratorsPerPage = 10;
                int skip = ((int)PageNumber - 1) * administratorsPerPage;
                query = query.Skip(skip).Take(administratorsPerPage);
            }

            return [.. query];
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            Administrator? administrator = _context.Administrators.Where(
                administrator => administrator.Email == loginDTO.Email && administrator.Password == loginDTO.Password
                ).FirstOrDefault();

            return administrator;
        }

        public void Update(Administrator administrator)
        {
            _context.Administrators.Update(administrator);
            _context.SaveChanges();
        }
    }
}