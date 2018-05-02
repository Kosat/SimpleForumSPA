namespace Api.Auth
{

	public class BuiltInUsersSettings
	{
		public BuiltInUserSettings[] Users { get; set;}
	}

	public class BuiltInUserSettings
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }
	}

	public class EndpointsSettings
	{
		public string Api_Auth { get; set; }
		public string Spa { get; set; }
		public string Spa_External { get; set; }
	}
}