// Models/Claim.cs - Paballo Nthutang ST10446382 - FINAL VERSION
using System.ComponentModel.DataAnnotations;

namespace CMCS_Paballo_Nthutang_ST10446382.Models
{
    public class Claim
    {
        public int Id { get; set; }

        // The user who submitted the claim (identity user id)
       
        public string UserId { get; set; } = string.Empty;

        // Navigation to the lecturer user (optional)
        public virtual AppUser? Lecturer { get; set; }

        // Required numeric fields
        [Required]
        public decimal HoursWorked { get; set; }

        [Required]
        public decimal HourlyRate { get; set; }

        // Computed total
        public decimal TotalAmount => HoursWorked * HourlyRate;

        public string? Notes { get; set; }

        // Default to UTC now; can be overridden when creating a claim
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

        // Status values: Pending, CoordinatorApproved, Approved, Rejected
        [Required]
        public string Status { get; set; } = "Pending";

        // Supporting documents collection
        public virtual ICollection<SupportingDocument> Documents { get; set; } = new List<SupportingDocument>();
    }

    public class SupportingDocument
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public virtual Claim? Claim { get; set; }

        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
}