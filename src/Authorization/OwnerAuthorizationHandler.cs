using Wish.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace Wish.Authorization;

public class OwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, WishListItem>
{
    private readonly UserManager<IdentityUser> _userManager;

    public OwnerAuthorizationHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        WishListItem resource)
    {
        if (context.User == null || resource == null)
        {
            return Task.CompletedTask;
        }

        if (Operations.CRUD.All(operation => operation.Name != requirement.Name))
        {
            return Task.CompletedTask;
        }

        if (resource.OwnerID == _userManager.GetUserId(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}