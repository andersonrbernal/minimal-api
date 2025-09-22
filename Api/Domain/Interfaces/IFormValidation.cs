using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.ModelViews;

namespace MinimalApi.Domain.Interfaces
{
    public interface IFormValidation
    {
        ValidationErrorModelView Validate(AdministratorDTO administratorDTO);
    }
}