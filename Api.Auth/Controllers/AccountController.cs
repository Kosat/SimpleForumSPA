using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Auth.Data;
using Api.Auth.Services;
using Api.Auth.ViewModels;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.Auth.Controllers
{
	public class AccountController : Controller
	{
		private const string ADMIN_ROLE = "admin";
		private const string MODERATOR_ROLE = "moderator";
		private const string USER_ROLE = "user";

		private readonly ApplicationDbContext _applicationDbContext;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ILogger _logger;
		private readonly IIdentityServerInteractionService _interaction;
		private readonly RedirectService _redirectService;

		public AccountController(
			ApplicationDbContext applicationDbContext,
			UserManager<ApplicationUser> userManager,
			IPersistedGrantService persistedGrantService,
			SignInManager<ApplicationUser> signInManager,
			ILoggerFactory loggerFactory,
			IIdentityServerInteractionService interaction,
			IClientStore clientStore,
			RedirectService redirectService
			)
		{
			_applicationDbContext = applicationDbContext;
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = loggerFactory.CreateLogger<AccountController>();
			_interaction = interaction;
			_redirectService = redirectService;
		}

		//
		// GET: /Account/Register
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register(string returnUrl = null)
		{
			ViewBag.ReturnUrl = returnUrl;
			ViewBag.AvailableRoles = new[] { USER_ROLE, MODERATOR_ROLE, ADMIN_ROLE };
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
		{
			ViewBag.ReturnUrl = returnUrl;
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email};
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					switch (model.Role)
					{
						case ADMIN_ROLE:
							await _userManager.AddToRoleAsync(user, USER_ROLE);
							await _userManager.AddToRoleAsync(user, MODERATOR_ROLE);
							await _userManager.AddToRoleAsync(user, ADMIN_ROLE);
							break;
						case MODERATOR_ROLE:
							await _userManager.AddToRoleAsync(user, USER_ROLE);
							await _userManager.AddToRoleAsync(user, MODERATOR_ROLE);
							break;
						case USER_ROLE:
							await _userManager.AddToRoleAsync(user, USER_ROLE);
							break;
					}

					await _signInManager.SignInAsync(user, isPersistent: false);
					_logger.LogInformation("User created a new account with password.");
					if (!string.IsNullOrWhiteSpace(returnUrl))
						return RedirectToLocal(returnUrl);
					else
						return RedirectToAction(nameof(Login));
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			ViewBag.ReturnUrl = returnUrl;
			ViewBag.AvailableRoles = new[] { USER_ROLE, MODERATOR_ROLE, ADMIN_ROLE };
			return View();
		}

		//
		// GET: /Account/Login
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login(string returnUrl = null)
		{
			var vm = new LoginViewModel
			{
				ReturnUrl = returnUrl,
			};
			return View(vm);
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			var returnUrl = _redirectService.ExtractRedirectUriFromReturnUrl(model.ReturnUrl);
			ViewBag.ReturnUrl = returnUrl;
			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberLogin, lockoutOnFailure: false);
				if (result.Succeeded)
				{
					_logger.LogInformation("User logged in.");
					if (!string.IsNullOrWhiteSpace(returnUrl))
						return Redirect(model.ReturnUrl);
					else
						return RedirectToAction(nameof(UserInfo));
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return View(model);
				}
			}

			// If something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/Logout
		[HttpGet]
		public async Task<IActionResult> Logout(string logoutId)
		{
			return await Logout(new LogoutViewModel { LogoutId = logoutId });
		}

		//
		// POST: /Account/Logout
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout(LogoutViewModel model)
		{

			// WROKAROUND: For some reason SignOutAsync doesn't delete cookies.
			Response.Cookies.Delete(".AspNetCore.Identity.Application");

			await HttpContext.SignOutAsync();

			// guest user
			HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

			var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);

			return Redirect(logout?.PostLogoutRedirectUri ?? "/logout");
		}

		//
		// GET: /Account/UserInfo
		[HttpGet]
		public async Task<IActionResult> UserInfo()
		{
			var user = await _userManager.GetUserAsync(User);
			ViewBag.IsLoggedIn = false;
			if (user != null)
			{
				var roles = await UserRolesAsCSVStr(user);
				ViewBag.IsLoggedIn = true;
				ViewBag.UserEmail = user.Email;
				ViewBag.UserRoles = roles;
			}

			return View();
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Register");
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Users()
		{
			var listUsers = _applicationDbContext.Users.ToList();
			var usersVm = new List<UserViewModel>();
			foreach (var user in listUsers)
			{
				var userRolesStr = await UserRolesAsCSVStr(user);

				usersVm.Add(new UserViewModel
				{
					Id = user.Id,
					Name = user.Email,
					Roles = userRolesStr
				});
			}
			return Ok(usersVm);
		}

		private async Task<string> UserRolesAsCSVStr(ApplicationUser user)
		{
			var userRoles = await _userManager.GetRolesAsync(user);
			var userRolesStr = string.Empty;
			if (userRoles != null)
			{
				userRolesStr = string.Join(',', userRoles);
			}

			return userRolesStr;
		}
	}
}
