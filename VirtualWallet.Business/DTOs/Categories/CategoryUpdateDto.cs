using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.Business.DTOs.Categories;

public class CategoryUpdateDto
{
    [Required] 
    [StringLength(50, ErrorMessage = "Category name cannot be longer than 50 characters.")]
    public string Name { get; set; } = null!;
}