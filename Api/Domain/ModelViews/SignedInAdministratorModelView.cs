using MinimalApi.Domain.DTOs.Enums;

namespace MinimalApi.Domain.ModelViews
{
    public record SignedInAdministratorModelView
    {
        public int Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Token { get; set; } = default!;
    }
}