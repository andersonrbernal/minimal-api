using System.ComponentModel;

namespace MinimalApi.Domain.DTOs.Enums
{
    public enum Profile
    {
        [Description(nameof(ADMINISTRATOR))]
        ADMINISTRATOR,
        [Description(nameof(EDITOR))]
        EDITOR
    }
}