// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using FlashcardsApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Security.Claims;
using FlashcardsApp.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FlashcardsApp.Models;

namespace FlashcardsApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly Uri _baseAddress = new("https://localhost:7296/api");
        private readonly HttpClient _httpClient;

        public RegisterModel()
        {
            _httpClient = new HttpClient() { BaseAddress = _baseAddress};
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [DisplayName("First name")]
            public string FirstName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [DisplayName("Last name")]
            public string LastName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        // Make this the create user api call
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                HttpResponseMessage res = HttpApiService.PostToAPI(_httpClient, "/Authenticate/Register", new { Input.FirstName, Input.LastName, Username = Input.Email, Input.Email, Input.Password });

                if (!res.IsSuccessStatusCode)
                {
                    // Redirect to registration
                    ModelState.AddModelError(string.Empty, "User already exists");
                    return Page();
                }

                // Login the user
                HttpResponseMessage loginRes = HttpApiService.PostToAPI(_httpClient, "/Authenticate/login", new {Username = Input.Email, Input.Password});
                TokenDataModel? tokenDataModel = ObjectSerialiser.Deserialise<TokenDataModel>(loginRes);

                if (!loginRes.IsSuccessStatusCode || tokenDataModel == null || tokenDataModel.Token == null || tokenDataModel.Id == null)
                {
                    return Page();
                }

                Response.Cookies.Delete("token");
                Response.Cookies.Delete("id");

                Response.Cookies.Append("token", tokenDataModel.Token, new CookieOptions() { HttpOnly = true });

                Response.Cookies.Append("id", tokenDataModel.Id);

                return LocalRedirect(returnUrl);
            }
            
            // If we got this far, something failed, redisplay form
            return Page();
        }

    }
}
