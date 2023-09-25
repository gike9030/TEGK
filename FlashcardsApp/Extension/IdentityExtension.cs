using System.Security.Claims;
using System.Security.Principal;
using FlashcardsApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;

namespace FlashcardsApp.Extension
{
    public static class IdentityExtension
    {
        public static string? GetFirstName(this UserManager<FlashcardsAppUser> UserManager, ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal.FindFirstValue("FirstName");
        }
    }
}
