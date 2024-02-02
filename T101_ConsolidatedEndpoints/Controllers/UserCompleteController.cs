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

		// Get
		[HttpGet("{userId}/{isActive}")]
		public IEnumerable<UserComplete> GetUser(int userId, bool isActive)
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

		// Upsert
		[HttpPut]
		public IActionResult UpsertUser(UserComplete userComplete)
		{
			string sql = @$"
			EXEC TutorialAppSchema.spUser_Upsert
				@FirstName = '{userComplete.FirstName}',
				@LastName = '{userComplete.LastName}',
				@Email = '{userComplete.Email}',
				@Gender = '{userComplete.Gender}',
				@Active = '{userComplete.Active}',
				@JobTitle = '{userComplete.JobTitle}',
				@Department = '{userComplete.Department}',
				@Salary = '{userComplete.Salary.ToString(_specifier, _culture)}',
				@UserId = {userComplete.UserId}
			";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			return StatusCode(400, "Failed to Upsert User.");
		}

		// Delete
		[HttpDelete("{userId}")]
		public IActionResult DeleteUser(int userId)
		{
			string sql = $"EXEC TutorialAppSchema.spUser_Delete @UserId={userId}";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			return StatusCode(400, "Failed to Delete User.");
		}
	}
}
