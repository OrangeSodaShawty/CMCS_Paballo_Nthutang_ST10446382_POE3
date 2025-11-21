// Models/ErrorViewModel.cs - Paballo Nthutang ST10446382
namespace CMCS_Paballo_Nthutang_ST10446382.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public Exception? Exception { get; set; }
    }
}