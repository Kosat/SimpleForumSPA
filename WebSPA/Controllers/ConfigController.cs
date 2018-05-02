using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Serilog;

namespace WebSPA.Controllers
{
	[Route("/config")]
	public class ConfigController : ControllerBase
	{
		private readonly IOptionsSnapshot<EndpointsSettings> _settings;
		private readonly ILogger<ConfigController> _logger;

		public ConfigController(ILoggerFactory loggerFactory,IOptionsSnapshot<EndpointsSettings> settings)
		{
			_settings = settings;
			_logger = loggerFactory.CreateLogger<ConfigController>();
		}

		public IActionResult Get()
		{
			// Keep it in sync with appsettings.endpoints.json
			// It intentailly repacks settings into json, rather than serving json file,
			// to enable environment variables.
			var spaConfig =
				new JObject(
					new JProperty("Endpoints",
						new JObject(
							new JProperty(nameof(_settings.Value.Api_Auth), _settings.Value.Api_Auth),
							new JProperty(nameof(_settings.Value.Spa), _settings.Value.Spa)
						)));
			_logger.LogWarning("Returning " + _settings.Value.Api_Auth + " and " + _settings.Value.Spa);
			return Content(spaConfig.ToString(), "application/json");
		}
	}
}
