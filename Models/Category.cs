
using System.ComponentModel.DataAnnotations;

namespace CrumbCount2.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Display(Name = "Category Name")]
        [Required(ErrorMessage = "Please enter the category name.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = "";

        [Display(Name = "Description")]
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
