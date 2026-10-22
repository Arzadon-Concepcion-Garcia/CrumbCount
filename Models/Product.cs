using System.ComponentModel.DataAnnotations;

namespace CrumbCount2.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = "";

    [Range(0, double.MaxValue)]
    public decimal UnitCost { get; set; }

    public bool IsActive { get; set; } = true;

    public Category? Category { get; set; }
}