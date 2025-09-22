namespace MinimalApi.Domain.DTOs
{
    public record VehicleDTO
    {
        public string Name { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public int Year { get; set; }
    }
}