// Controllers/ClaimsController.cs - Paballo Nthutang ST10446382
using ClosedXML.Excel;
using CMCS_Paballo_Nthutang_ST10446382.Data;
using CMCS_Paballo_Nthutang_ST10446382.Hubs;
using CMCS_Paballo_Nthutang_ST10446382.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ClaimModel = CMCS_Paballo_Nthutang_ST10446382.Models.Claim;

[Authorize]
public class ClaimsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IHubContext<ClaimHub> _hub;

    public ClaimsController(ApplicationDbContext context, UserManager<AppUser> userManager, IHubContext<ClaimHub> hub)
    {
        _context = context;
        _userManager = userManager;
        _hub = hub;
    }

    // Lecturer: Submit
    [Authorize(Roles = "Lecturer")]
    public IActionResult Submit() => View(new Claim());

    [HttpPost]
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> Submit(Claim claim, List<IFormFile> documents)
    {
        if (ModelState.IsValid)
        {
            claim.UserId = _userManager.GetUserId(User)!;
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadPath);

            foreach (var file in documents)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var fullPath = Path.Combine(uploadPath, fileName);
                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    _context.SupportingDocuments.Add(new SupportingDocument
                    {
                        ClaimId = claim.Id,
                        OriginalFileName = file.FileName,
                        StoredFileName = fileName,
                        FileSize = file.Length
                    });
                }
            }
            await _context.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("StatusChanged", claim.Id, "Pending");
            return RedirectToAction("MyClaims");
        }
        return View(claim);
    }

    // Lecturer: My Claims
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> MyClaims()
    {
        var userId = _userManager.GetUserId(User);
        var claims = await _context.Claims.Include(c => c.Documents)
            .Where(c => c.UserId == userId).ToListAsync();
        return View(claims);
    }

    // Coordinator Dashboard
    [Authorize(Roles = "Coordinator")]
    public async Task<IActionResult> Coordinator()
    {
        var claims = await _context.Claims
            .Include(c => c.Lecturer)
            .Include(c => c.Documents)
            .Where(c => c.Status == "Pending")
            .ToListAsync();

        return View(claims);
    }

    // Manager Dashboard
    [Authorize(Roles = "AcademicManager")]
    public async Task<IActionResult> Manager()
    {
        var claims = await _context.Claims
            .Include(c => c.Lecturer)
            .Include(c => c.Documents)
            .Where(c => c.Status == "CoordinatorApproved")
            .ToListAsync();

        return View(claims);
    }


    // HR Dashboard + Export
    [Authorize(Roles = "HR")]
    public async Task<IActionResult> HR()
    {
        var claims = await _context.Claims.Where(c => c.Status == "Approved").ToListAsync();
        return View(claims);
    }

    [Authorize(Roles = "HR")]
    public async Task<IActionResult> ExportExcel()
    {
        var claims = await _context.Claims.Where(c => c.Status == "Approved").ToListAsync();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Approved Claims");
        ws.Cell(1, 1).Value = "Lecturer";
        ws.Cell(1, 2).Value = "Hours";
        ws.Cell(1, 3).Value = "Rate";
        ws.Cell(1, 4).Value = "Total";
        ws.Cell(1, 5).Value = "Date";

        for (int i = 0; i < claims.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = claims[i].Lecturer?.FullName;
            ws.Cell(i + 2, 2).Value = claims[i].HoursWorked;
            ws.Cell(i + 2, 3).Value = claims[i].HourlyRate;
            ws.Cell(i + 2, 4).Value = claims[i].TotalAmount;
            ws.Cell(i + 2, 5).Value = claims[i].SubmittedDate.ToShortDateString();
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"CMCS_ApprovedClaims_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    // Shared Actions
    [HttpPost]
    [Authorize(Roles = "Coordinator,AcademicManager")]
    public async Task<IActionResult> Approve(int id, string? comment)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim != null)
        {
            var isCoordinator = User.IsInRole("Coordinator");
            claim.Status = isCoordinator ? "CoordinatorApproved" : "Approved";
            await _context.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("StatusChanged", id, claim.Status.ToString());
        }
        return RedirectToAction(User.IsInRole("Coordinator") ? "Coordinator" : "Manager");
    }

    [HttpPost]
    [Authorize(Roles = "Coordinator,AcademicManager")]
    public async Task<IActionResult> Reject(int id, string? comment)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim != null)
        {
            claim.Status = "Rejected";
            await _context.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("StatusChanged", id, "Rejected");
        }
        return RedirectToAction(User.IsInRole("Coordinator") ? "Coordinator" : "Manager");
    }
}