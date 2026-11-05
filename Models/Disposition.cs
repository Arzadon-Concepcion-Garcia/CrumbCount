
using System;
using System.ComponentModel.DataAnnotations;

namespace CrumbCount2.Models;

public class Disposition
{
    public int DispositionId { get; set; }

    [Required(ErrorMessage = "Please select a waste recording.")]
    public int WasteRecordingId { get; set; }

    [Required(ErrorMessage = "Please enter a disposition type.")]
    [StringLength(50, ErrorMessage = "Disposition type cannot exceed 50 characters.")]
    [RegularExpression("^(Donation|Recycling|Composting|Disposal)$",
        ErrorMessage = "Select a valid disposition type.")]
    [Display(Name = "Disposition Type")]
    public string DispositionType { get; set; } = "";

    [Required(ErrorMessage = "Please enter a quantity.")]
    [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100,000.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Please enter a disposition date.")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(Disposition), nameof(ValidateDispositionDate))]
    public DateTime DispositionDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Please select a status.")]
    [RegularExpression("^(Pending|Completed|Cancelled)$",
        ErrorMessage = "Select a valid status.")]
    public string Status { get; set; } = "Pending";

    [StringLength(250, ErrorMessage = "Remarks cannot exceed 250 characters.")]
    public string? Remarks { get; set; }

    public WasteRecording? WasteRecording { get; set; }

    public static ValidationResult? ValidateDispositionDate(DateTime date)
    {
        if (date.Date > DateTime.Today)
        {
            return new ValidationResult("Disposition date cannot be in the future.");
        }

        return ValidationResult.Success;
    }
}
