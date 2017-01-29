using Heisenslaught.Infrastructure.MongoDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;


namespace Heisenslaught.Users
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly UserManager<HSUser> _userManager;
        private readonly SignInManager<HSUser> _signInManager;
        private readonly UserCreationSettings _userCreationSettings;

        public AuthController(
            UserManager<HSUser> userManager,
            SignInManager<HSUser> signInManager,
            IOptions<UserCreationSettings> userCreationSettings
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userCreationSettings = userCreationSettings.Value;
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

        [HttpGet("auth/switch")]
        public async Task<IActionResult> SwitchUserAsync(string provider, string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            ViewData["loginUrl"] = "/auth?provider=" + provider + "&returnUrl=" + returnUrl;
            return View("LoginSwitch");
        }

        [HttpGet]
        [AllowAnonymous]
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
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
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
                            if (_userCreationSettings.AutoGrantSuperUserToBattleTags.Contains(user.BattleTag))
                            {
                                await _userManager.AddToRoleAsync(user, "su");
                            }

                            await _signInManager.SignInAsync(user, isPersistent: true);
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

        [HttpGet("auth/user")]
        [AllowAnonymous]
        public async Task<AuthenticatedUserDTO> GetAuthenticatedUserAsync()
        {
            if (User != null)
            {
                HSUser user = await _userManager.GetUserAsync(this.User);
                if(user != null)
                {
                    return new AuthenticatedUserDTO(user);
                }
            }
            return null;
        }

    }
}
