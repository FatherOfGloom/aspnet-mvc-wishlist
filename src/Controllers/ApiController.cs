using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wish.Models;

namespace Wish.Controllers;

[Route("/api")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly IWishListRepository _repository;
    private readonly UserManager<IdentityUser> _userManager;

    public ApiController(
        IWishListRepository repository,
        UserManager<IdentityUser> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    [HttpGet]
    public ActionResult<IEnumerable<WishListItem>> GetApi()
    {
        var currentUserID = _userManager.GetUserId(User);

        var items = _repository
            .GetWishListItems()
            .Where(item => item.OwnerID == currentUserID);

        return Ok(items);
    }
}