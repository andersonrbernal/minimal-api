using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.ModelViews;

namespace MinimalApi.Domain.FormValidation
{
    public static class VehicleValidation
    {
        private static ValidationErrorModelView validation = new();

        public static ValidationErrorModelView Validate(VehicleDTO vehicleDTO)
        {
            validation = new();

            if (string.IsNullOrEmpty(vehicleDTO.Name))
                validation.Messages.Add("The vehicle name cannot be empty.");

            if (string.IsNullOrEmpty(vehicleDTO.Brand))
                validation.Messages.Add("The vehicle brand cannot be empty.");

            if (vehicleDTO.Year < 1900)
                validation.Messages.Add("Vehicle too old, only vehicles from 1900 onwards are accepted.");

            return validation;
        }
    }
}