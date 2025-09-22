using System.Linq;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.DTOs.Enums;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;

namespace MinimalApi.Domain.FormValidation
{
    public static class AdministratorValidation
    {
        private static ValidationErrorModelView validation = new();

        public static ValidationErrorModelView Validate(AdministratorDTO administratorDTO)
        {
            if (string.IsNullOrEmpty(administratorDTO.Email))
                validation.Messages.Add("Email is required");

            if (string.IsNullOrWhiteSpace(administratorDTO.Password))
                validation.Messages.Add("Password is required");
            else if (administratorDTO.Password.Length < 8)
                validation.Messages.Add("Password should be at least 8 characters in length.");

            return validation;
        }
    }
}