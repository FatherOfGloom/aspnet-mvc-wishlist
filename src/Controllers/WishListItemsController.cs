using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wish.Authorization;
using Wish.Models;

namespace Wish.Controllers;

public class WishListItemsController : Controller
{
    private readonly IWishListRepository _repository;
    private readonly IWebHostEnvironment _environment;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAuthorizationService _authorizationService;

    public WishListItemsController(
        IWishListRepository repository,
        IWebHostEnvironment environment,
        UserManager<IdentityUser> userManager,
        IAuthorizationService authorizationService)
    {
        _repository = repository;
        _environment = environment;
        _userManager = userManager;
        _authorizationService = authorizationService;
    }

    public IActionResult Index() => Redirect("/");

    public async Task<IActionResult> Details(int? id)
        => await GetViewAsync(id, Operations.Read);

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Image")] WishListItem item)
    {
        if (!ModelState.IsValid)
        {
            return View(item);
        }

        if (item.Image != null)
        {
            var fileName = Guid.NewGuid().ToString() + "-" + item.Name + ".jpg";
            var filePath = Path.Combine(_environment.WebRootPath,
                "data",
                fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await item.Image.CopyToAsync(stream);
            }

            item.ImageURL = $"/data/{fileName}";
        }

        item.OwnerID = _userManager.GetUserId(User);
        item.Acquired = DateTime.Now;
        item.DescriptionHeaders = new List<string> { "" };
        item.DescriptionBodies = new List<string> { "" };

        _repository.AppendItem(item);
        await _repository.SaveAsync();
        return Redirect("/");
    }

    public async Task<IActionResult> Delete(int? id)
        => await GetViewAsync(id, Operations.Delete);

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_repository.GetWishListItems().Count() == 0)
        {
            return Problem("Entity set 'CollectionContext.CollectionItems'  is null.");
        }
        var item = await _repository.FindByIdAsync(id);
        var filePath = item.ImageURL;

        if (filePath != null)
        {
            filePath = Path
                .Combine(
                    Directory
                    .GetCurrentDirectory(),
                    "wwwroot\\data",
                    filePath?.Split('/').Last()
                    ?? string.Empty
                );

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        await _repository.DeleteByIdAsync(id);

        await _repository.SaveAsync();
        return Redirect("/");
    }
    public async Task<IActionResult> Edit(int? id)
        => await GetViewAsync(id, Operations.Update);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,Name,Acquired,ImageURL,DescriptionBodies,DescriptionHeaders,OwnerID")]
        WishListItem item)
    {
        if (id != item.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(item);
        }

        try
        {
            _repository.UpdateItem(item);
            await _repository.SaveAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_repository.ItemExists(item.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> GetViewAsync(
        int? id,
        OperationAuthorizationRequirement requirement)
    {
        if (id == null)
        {
            return NotFound();
        }

        var item = await _repository.FindByIdAsync((int)id);
        if (item == WishListItem.NotFound)
        {
            return NotFound();
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, item, requirement);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        return View(item);
    }
}