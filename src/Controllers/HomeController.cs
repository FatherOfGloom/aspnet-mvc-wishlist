using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wish.Models;

namespace Wish.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWishListRepository _repository;
    private readonly UserManager<IdentityUser> _userManager;
    public HomeController(
        ILogger<HomeController> logger, 
        IWishListRepository repository, 
        UserManager<IdentityUser> userManager)
    {
        _repository = repository;
        _logger = logger;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var currentUserID = _userManager.GetUserId(User);

        var items = _repository
            .GetWishListItems()
            .Where(item => item.OwnerID == currentUserID);

        return View(items);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode = null)
    {
        if (statusCode.HasValue)
        {
            return View("StatusCode", new ErrorViewModel { StatusCode = statusCode });
        }

        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        ViewBag.StackTrace = exceptionHandlerPathFeature?.Error.StackTrace;
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
