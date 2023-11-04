using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Authorization
{
    public class AppUserAuthorizeAttribute : TypeFilterAttribute
    {
        public AppUserAuthorizeAttribute() : base(typeof(AppUserAuthorizeFilter))
        {
        }
    }
}
