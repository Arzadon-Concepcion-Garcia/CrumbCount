using CrumbCount2.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrumbCount2.Models
{
    public class WasteRecording
    {
        public int WasteRecordingId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a valid product.")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please enter the quantity wasted.")]
        [Range(1, 100000,
            ErrorMessage = "Quantity must be between 1 and 100,000.")]
        [Display(Name = "Quantity Wasted")]
        public int QuantityWasted { get; set; }

        
        [Required(ErrorMessage = "Please select a waste date.")]
        [DataType(DataType.Date)]
        [NotFutureDate]
        [Display(Name = "Date Recorded")]
        public DateTime DateRecorded { get; set; } = DateTime.Today;


        [Required(ErrorMessage = "Please enter a reason for waste.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Reason must be between 2 and 100 characters.")]
        [Display(Name = "Reason for Waste")]
        public string ReasonForWaste { get; set; } = "";

        [StringLength(250,
            ErrorMessage = "Remarks cannot exceed 250 characters.")]
        public string? Remarks { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [NotMapped]
        [Display(Name = "Estimated Waste Cost")]
        public decimal EstimatedWasteCost =>
            QuantityWasted * (Product?.UnitCost ?? 0);
    }
}
