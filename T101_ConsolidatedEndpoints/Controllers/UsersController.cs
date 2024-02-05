using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using T101_ConsolidatedEndpoints.Data;
using T101_ConsolidatedEndpoints.Models;

namespace T101_ConsolidatedEndpoints.Controllers
{
	//[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UsersController(IConfiguration config) : ControllerBase
	{
		private readonly string _specifier = "0.00";
		private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

		private readonly DataContextDapper _dapper = new(config);

		// Get
		[HttpGet("{userId}/{isActive}")]
		public IEnumerable<UserModel> GetUser(int userId, bool isActive)
		{
			string sql = "EXEC TutorialAppSchema.spUsers_Get";
			string stringParameters = "";
			DynamicParameters sqlParameters = new();

			if (userId != 0)
			{
				stringParameters += ", @UserId=@UserIdParam";
				sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
			}

			if (isActive)
			{
				stringParameters += ", @Active=@ActiveParam";
				sqlParameters.Add("@ActiveParam", isActive, DbType.Boolean);
			}

			sql += stringParameters[1..];

			IEnumerable<UserModel> users = _dapper.LoadDataWithParameters<UserModel>(sql, sqlParameters);

			return users;
		}

		// Upsert
		[HttpPut]
		public IActionResult UpsertUser(UserModel userComplete)
		{
			string sql = @"
			EXEC TutorialAppSchema.spUser_Upsert
				@FirstName = @FirstNameParam,
				@LastName = @LastNameParam,
				@Email = @EmailParam,
				@Gender = @GenderParam,
				@Active = @ActiveParam,
				@JobTitle = @JobTitleParam,
				@Department = @DepartmentParam,
				@Salary = @SalaryParam,
				@UserId = @UserIdParam
			";
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@FirstNameParam", userComplete.FirstName, DbType.String);
			sqlParameters.Add("@LastNameParam", userComplete.LastName, DbType.String);
			sqlParameters.Add("@EmailParam", userComplete.Email, DbType.String);
			sqlParameters.Add("@GenderParam", userComplete.Gender, DbType.String);
			sqlParameters.Add("@ActiveParam", userComplete.Active, DbType.Boolean);
			sqlParameters.Add("@JobTitleParam", userComplete.JobTitle, DbType.String);
			sqlParameters.Add("@DepartmentParam", userComplete.Department, DbType.String);
			sqlParameters.Add("@SalaryParam", userComplete.Salary, DbType.Decimal);
			sqlParameters.Add("@UserIdParam", userComplete.UserId, DbType.Int32);

			if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
			{
				return Ok();
			}

			return StatusCode(400, "Failed to Upsert User.");
		}

		// Delete
		[HttpDelete("{userId}")]
		public IActionResult DeleteUser(int userId)
		{
			string sql = $"EXEC TutorialAppSchema.spUser_Delete @UserId=@UserIdParam";
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
