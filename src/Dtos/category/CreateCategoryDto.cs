using System.ComponentModel.DataAnnotations;

namespace src.Dtos.category
{
    public class CreateCategoryDto
    {
        [StringLength(45, MinimumLength = 3)]
        public string Label { get; set; } = string.Empty;
    }
}