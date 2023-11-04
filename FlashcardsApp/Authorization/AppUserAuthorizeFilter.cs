using FlashcardsApp.Models;
using System.Net.Http;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlashcardsApp.Authorization
{
    public class AppUserAuthorizeFilter : Attribute, IAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Uri _baseAddress = new("https://localhost:7296/api");
        private readonly HttpClient _httpClient;

        public AppUserAuthorizeFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = new HttpClient() { BaseAddress = _baseAddress};
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!IsAuthorized(context))
            {
                context.Result = new RedirectResult("/Identity/Account/Login/?ReturnUrl=%2F");
            }
        }

        private bool IsAuthorized(AuthorizationFilterContext _)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.Request.Cookies["id"] != null && httpContext.Request.Cookies["token"] != null)
            {
                Response? name = HttpApiService.GetFromAPI<Response>(_httpClient, "/FlashcardsAppUser/GetFirstName/" + httpContext.Request.Cookies["id"], token: httpContext.Request.Cookies["token"]);

                if (name == null)
                {
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}
