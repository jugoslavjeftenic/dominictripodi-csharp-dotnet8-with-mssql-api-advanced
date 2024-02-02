using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using T101_ConsolidatedEndpoints.Data;
using T101_ConsolidatedEndpoints.Models;

namespace T101_ConsolidatedEndpoints.Controllers
{
	//[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UserCompleteController(IConfiguration config) : ControllerBase
	{
		private readonly string _specifier = "0.00";
		private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

		private readonly DataContextDapper _dapper = new(config);

		// UserModel -------------------------------------------

		// Read
		[HttpGet("{userId}/{isActive}")]
		public IEnumerable<UserComplete> GetUsersByParam(int userId, bool isActive)
		{
			string sql = "EXEC TutorialAppSchema.spUsers_Get";
			string parameters = "";

			if (userId != 0)
			{
				parameters += $", @UserId={userId}";
			}

			if (isActive)
			{
				parameters += $", @Active={isActive}";
			}

			sql += parameters[1..];

			IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);

			return users;
		}
	}
}
