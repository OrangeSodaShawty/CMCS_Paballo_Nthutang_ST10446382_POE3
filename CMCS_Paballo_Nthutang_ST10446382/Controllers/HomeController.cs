using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMCS_Paballo_Nthutang_ST10446382.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // Redirects users to the correct dashboard after login
        public IActionResult Index()
        {
            if (User.IsInRole("Lecturer"))
                return RedirectToAction("LecturerHome");

            if (User.IsInRole("Coordinator"))
                return RedirectToAction("CoordinatorHome");

            if (User.IsInRole("AcademicManager"))
                return RedirectToAction("ManagerHome");

            if (User.IsInRole("HR"))
                return RedirectToAction("HRHome");

            // Default fallback view (not normally shown)
            return View();
        }

        // ------------------- Lecturer Dashboard -------------------
        [Authorize(Roles = "Lecturer")]
        public IActionResult LecturerHome()
        {
            return View();
        }

        // ------------------- Coordinator Dashboard -------------------
        [Authorize(Roles = "Coordinator")]
        public IActionResult CoordinatorHome()
        {
            return View();
        }

        // ------------------- Academic Manager Dashboard -------------------
        [Authorize(Roles = "AcademicManager")]
        public IActionResult ManagerHome()
        {
            return View();
        }

        // ------------------- HR Dashboard -------------------
        [Authorize(Roles = "HR")]
        public IActionResult HRHome()
        {
            return View();
        }

        // ------------------- Privacy Page -------------------
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            if (User.IsInRole("Lecturer"))
                return RedirectToAction("LecturerHome");

            if (User.IsInRole("Coordinator"))
                return RedirectToAction("CoordinatorHome");

            if (User.IsInRole("AcademicManager"))
                return RedirectToAction("ManagerHome");

            if (User.IsInRole("HR"))
                return RedirectToAction("HRHome");

            return RedirectToAction("Index");
        }

    }
}
