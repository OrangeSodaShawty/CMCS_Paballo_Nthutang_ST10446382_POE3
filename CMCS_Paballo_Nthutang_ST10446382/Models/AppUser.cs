using Microsoft.AspNetCore.Identity;

namespace CMCS_Paballo_Nthutang_ST10446382.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = "Paballo Nthutang";
    }
}