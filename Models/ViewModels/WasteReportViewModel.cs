
using System.Collections.Generic;
using CrumbCount2.Models;

namespace CrumbCount2.Models.ViewModels
{
    public class WasteReportViewModel
    {
        public int TotalWasteRecords { get; set; }

        public int TotalQuantityWasted { get; set; }

        public decimal TotalWasteCost { get; set; }

        public string AiInsights { get; set; } = "";

        // Stores individual waste records, including their dates
        public List<WasteRecording> WasteRecords { get; set; }
            = new List<WasteRecording>();

        public List<ProductWasteSummary> ProductSummaries { get; set; }
            = new List<ProductWasteSummary>();

        public List<CategoryWasteSummary> CategorySummaries { get; set; }
            = new List<CategoryWasteSummary>();

        public List<ReasonWasteSummary> ReasonSummaries { get; set; }
            = new List<ReasonWasteSummary>();
    }

    public class ProductWasteSummary
    {
        public string ProductName { get; set; } = "";

        public int QuantityWasted { get; set; }

        public decimal WasteCost { get; set; }
    }

    public class CategoryWasteSummary
    {
        public string CategoryName { get; set; } = "";

        public decimal WasteCost { get; set; }
    }

    public class ReasonWasteSummary
    {
        public string Reason { get; set; } = "";

        public int QuantityWasted { get; set; }

        public decimal WasteCost { get; set; }
    }
}
