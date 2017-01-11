using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Heisenslaught.Models.Users;
using Newtonsoft.Json;
using Heisenslaught.DataTransfer.Users;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Heisenslaught.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly UserManager<HSUser> _userManager;
        private readonly SignInManager<HSUser> _signInManager;

        public AuthController(
            UserManager<HSUser> userManager,
            SignInManager<HSUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("CallbackAsync", "Auth", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        //https://localhost:44301/
        public async Task<IActionResult> CallbackAsync(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                ViewData["loginResult"] = new LoginResultDTO<string>(true, remoteError);
                return View("LoginEvent");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) // failed for some reson
            {
                if(this.User != null)
                {
                    HSUser user = await _userManager.GetUserAsync(this.User);
                     ViewData["loginResult"] = new LoginResultDTO<AuthenticatedUserDTO>(true, new AuthenticatedUserDTO(user));
                }
                return View("LoginEvent");
            }
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded) // logged in
            {
                // Update any authentication tokens if login succeeded
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                if (this.User != null)
                {
                    HSUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                    ViewData["loginResult"] = new LoginResultDTO<AuthenticatedUserDTO>(true, new AuthenticatedUserDTO(user));
                }

                // ViewData['loginEvent'] = new LoginResultDTO<AuthenticatedUserDTO>(true, new AuthenticatedUserDTO());
                return View("LoginEvent");
            }
            else // new account
            {
                if (info.LoginProvider == "BattleNet")
                {
                    var user = new HSUser(info.ProviderKey, info.Principal.Identity.Name);
                    var createResult = await _userManager.CreateAsync(user);
                    if (createResult.Succeeded)
                    {
                        createResult = await _userManager.AddLoginAsync(user, info);
                        if (createResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                            ViewData["loginResult"] = new LoginResultDTO<AuthenticatedUserDTO>(true, new AuthenticatedUserDTO(user));
                            return View("LoginEvent");
                        }
                    }
                }
                return View("LoginEvent");
            }
        }


        [HttpGet]
        public async Task<bool> Logout()
        {
            await _signInManager.SignOutAsync();
            return true;
        }

    }
}
