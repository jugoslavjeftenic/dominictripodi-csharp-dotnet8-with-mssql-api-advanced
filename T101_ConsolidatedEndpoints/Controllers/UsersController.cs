using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using T101_ConsolidatedEndpoints.Data;
using T101_ConsolidatedEndpoints.Helpers;
using T101_ConsolidatedEndpoints.Models;

namespace T101_ConsolidatedEndpoints.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UsersController(IConfiguration config) : ControllerBase
	{
		private readonly DataContextDapper _dapper = new(config);
		private readonly ReusableSql _reusableSql = new(config);

		// Get
		[HttpGet("{userId}/{isActive}")]
		public IEnumerable<UserModel> GetUser(int userId, bool isActive)
		{
			string sql = "EXEC TutorialAppSchema.spUsers_Get";
			string stringParameters = "";
			DynamicParameters sqlParameters = new();

			if (userId != 0)
			{
				stringParameters += ", @UserId = @UserIdParam";
				sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
			}

			if (isActive)
			{
				stringParameters += ", @Active = @ActiveParam";
				sqlParameters.Add("@ActiveParam", isActive, DbType.Boolean);
			}

			if (stringParameters.Length > 0)
			{
				sql += stringParameters[1..];
			}

			IEnumerable<UserModel> users = _dapper.LoadDataWithParameters<UserModel>(sql, sqlParameters);

			return users;
		}

		// Upsert
		[HttpPut]
		public IActionResult UpsertUser(UserModel user)
		{
			if (_reusableSql.UpsertUser(user))
			{
				return Ok();
			}

			return StatusCode(400, "Failed to Upsert User.");
		}

		// Delete
		[HttpDelete("{userId}")]
		public IActionResult DeleteUser(int userId)
		{
			string sql = $"EXEC TutorialAppSchema.spUser_Delete @UserId = @UserIdParam";
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@UserIdParam", userId, DbType.Int32);

			if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
			{
				return Ok();
			}

			return StatusCode(400, "Failed to Delete User.");
		}
	}
}
