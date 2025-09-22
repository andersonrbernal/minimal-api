using Mapster;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.DTOs.Enums;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;

namespace Test.Mocks.Services;

public class AdministratorServiceMock : IAdministratorService
{
    private static List<Administrator> administrators { get; set; } = [
        new() {Id = 1, Email = "administrator@minimal_api.com", Password ="12345678", Profile = Profile.ADMINISTRATOR},
        new() {Id = 2, Email = "editor@minimal_api.com", Password ="12345678", Profile = Profile.EDITOR}
    ];

    public Administrator Create(Administrator administrator)
    {
        administrator.Id = administrators.Count + 1;
        administrators.Add(administrator);
        return administrator;
    }

    public void Delete(Administrator administrator)
    {
        administrators.Remove(administrator);
    }

    public Administrator? GetAdministrator(int Id)
    {
        return administrators.Find(administrator => administrator.Id == Id);
    }

    public List<Administrator> GetAdministrators(int? PageNumber = 1)
    {
        var query = administrators.AsQueryable().AsNoTracking();

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
        return administrators.Find(administrator => administrator.Email == loginDTO.Email && administrator.Password == loginDTO.Password);
    }

    public void Update(Administrator administrator)
    {
        administrators
            .Where(a => a.Id == administrator.Id)
            .FirstOrDefault()
            .Adapt(administrator);
    }
}