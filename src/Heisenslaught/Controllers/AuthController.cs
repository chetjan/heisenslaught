using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Heisenslaught.Models.Users;
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
        //https://localhost:44301/auth?provider=BattleNet
        //https://localhost:44301/auth?provider=bnet
        // https://localhost:44301/auth?provider=google
        public IActionResult Index(string provider, string returnUrl = null)
        {
            returnUrl = "https://localhost:44301/";

            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("Callback", "Auth", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Callback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) // failed for some reson
            {
                return View("Login");
            }
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded) // logged in
            {
                // Update any authentication tokens if login succeeded
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                //_logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
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
                            return RedirectToLocal(returnUrl);
                        }
                    }
                }
                return View("Login");
            }
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return View("Login");
            }
        }

    }
}
