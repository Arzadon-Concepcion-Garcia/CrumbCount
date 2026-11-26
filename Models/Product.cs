
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrumbCount2.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Display(Name = "Category")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "Please enter the product name.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; } = "";

        [Display(Name = "Unit Cost")]
        [Range(0, double.MaxValue, ErrorMessage = "Unit cost cannot be negative.")]
        public decimal UnitCost { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}
