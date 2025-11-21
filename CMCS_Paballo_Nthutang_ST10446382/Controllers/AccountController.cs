using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CMCS_Paballo_Nthutang_ST10446382.Models;

public class AccountController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = "/")
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = "/")
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

        if (result.Succeeded)
            return Redirect(returnUrl ?? "/");

        ModelState.AddModelError("", "Invalid login attempt.");
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
