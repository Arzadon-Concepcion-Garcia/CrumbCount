
using System;
using System.ComponentModel.DataAnnotations;

namespace CrumbCount2.Models;

public class Disposition
{
    public int DispositionId { get; set; }

    [Required]
    [Display(Name = "Waste Recording")]
    public int WasteRecordingId { get; set; }

    public WasteRecording? WasteRecording { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Disposition Type")]
    public string DispositionType { get; set; } = "";

    [Required]
    [Range(1, 100000)]
    public int Quantity { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Disposition Date")]
    public DateTime DispositionDate { get; set; } = DateTime.Today;

    [StringLength(250)]
    public string? Remarks { get; set; }
}
