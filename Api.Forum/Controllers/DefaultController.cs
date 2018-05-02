using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Forum.Controllers
{
	[AllowAnonymous]
	[ApiExplorerSettings(IgnoreApi = true)] // Hide from swagger
	public class DefaultController : Controller
	{
		[Route("/")]
		public IActionResult Index()
		{
			return new RedirectResult("~/swagger/#");
		}
	}
}
