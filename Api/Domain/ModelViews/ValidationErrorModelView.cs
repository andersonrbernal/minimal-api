namespace MinimalApi.Domain.ModelViews
{
    public struct ValidationErrorModelView()
    {
        public List<string> Messages { get; set; } = [];
    }
}