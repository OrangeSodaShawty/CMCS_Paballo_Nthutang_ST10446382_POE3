// Data/ApplicationDbContext.cs - Paballo Nthutang ST10446382
using System.Security.Claims;
using CMCS_Paballo_Nthutang_ST10446382.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMCS_Paballo_Nthutang_ST10446382.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Fully qualify to avoid clash with System.Security.Claims.Claim
        public DbSet<CMCS_Paballo_Nthutang_ST10446382.Models.Claim> Claims { get; set; } = null!;
        public DbSet<SupportingDocument> SupportingDocuments { get; set; } = null!;
    }
}