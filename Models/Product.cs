
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrumbCount2.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = "";

        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
