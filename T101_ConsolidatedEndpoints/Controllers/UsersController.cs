using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using T101_ConsolidatedEndpoints.Data;
using T101_ConsolidatedEndpoints.Dtos;
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

		// UserModel -------------------------------------------
		// Create
		[HttpPost("AddUser")]
		public IActionResult AddUser(UserToAddDto user)
		{
			string sql = @$"
			INSERT INTO [TutorialAppSchema].[Users] (
				[FirstName],
				[LastName],
				[Email],
				[Gender],
				[Active]
			) VALUES (
				'{user.FirstName}',
				'{user.LastName}',
				'{user.Email}',
				'{user.Gender}',
				'{user.Active}'
			)";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Add User");
		}

		// Read - all
		[HttpGet("GetUsers")]
		public IEnumerable<UserModel> GetUsers()
		{
			string sql = @"
			SELECT
				[UserId],
				[FirstName],
				[LastName],
				[Email],
				[Gender],
				[Active]
			FROM [TutorialAppSchema].[Users]
			";

			IEnumerable<UserModel> users = _dapper.LoadData<UserModel>(sql);

			return users;
		}

		// Read - byId
		[HttpGet("GetUser/{userId}")]
		public UserModel GetUser(int userId)
		{
			string sql = @$"
			SELECT
				[UserId],
				[FirstName],
				[LastName],
				[Email],
				[Gender],
				[Active]
			FROM [TutorialAppSchema].[Users]
			WHERE [UserId] = {userId}
			";

			UserModel user = _dapper.LoadDataSingle<UserModel>(sql);

			return user;
		}

		// Update
		[HttpPut("EditUser")]
		public IActionResult EditUser(UserModel user)
		{
			string sql = @$"
			UPDATE [TutorialAppSchema].[Users] SET
				[FirstName] = '{user.FirstName}',
				[LastName] = '{user.LastName}',
				[Email] = '{user.Email}',
				[Gender] = '{user.Gender}',
				[Active] = '{user.Active}'
			WHERE UserId = {user.UserId}
			";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Update User");
		}

		// Delete
		[HttpDelete("DeleteUser/{userId}")]
		public IActionResult DeleteUser(int userId)
		{
			string sql = @$"
			DELETE
			FROM [TutorialAppSchema].[Users]
			WHERE [UserId] = {userId}
			";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Delete User");
		}

		// UserSalaryModel -------------------------------------
		// Create
		[HttpPost("AddUserSalary")]
		public IActionResult AddUserSalary(UserSalaryModel userSalary)
		{
			string sql = @$"
			INSERT INTO [TutorialAppSchema].[UserSalary] (
				[UserId],
				[Salary],
				[AvgSalary]
			) VALUES (
				'{userSalary.UserId}',
				'{userSalary.Salary.ToString(_specifier, _culture)}',
				'{userSalary.AvgSalary.ToString(_specifier, _culture)}'
			)";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Add User's Salary.");
		}

		// Read - byId
		[HttpGet("GetUserSalary/{userId}")]
		public IEnumerable<UserSalaryModel> GetUserSalary(int userId)
		{
			string sql = @$"
			SELECT
				[UserId],
				[Salary],
				[AvgSalary]
			FROM [TutorialAppSchema].[UserSalary]
			WHERE [UserId] = {userId}
			";

			IEnumerable<UserSalaryModel> userSalarys = _dapper.LoadData<UserSalaryModel>(sql);

			return userSalarys;
		}

		// Update
		[HttpPut("EditUserSalary")]
		public IActionResult EditUserSalary(UserSalaryModel userSalary)
		{
			string sql = @$"
			UPDATE [TutorialAppSchema].[UserSalary] SET
				[Salary] = '{userSalary.Salary.ToString(_specifier, _culture)}',
				[AvgSalary] = '{userSalary.AvgSalary.ToString(_specifier, _culture)}'
			WHERE UserId = {userSalary.UserId}
			";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Update User's Salary.");
		}

		// Delete
		[HttpDelete("DeleteUserSalary/{userId}")]
		public IActionResult DeleteUserSalary(int userId)
		{
			string sql = @$"
			DELETE
			FROM [TutorialAppSchema].[UserSalary]
			WHERE [UserId] = {userId}
			";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Delete User's Salary.");
		}

		// UserJobInfoModel ------------------------------------
		// Create
		[HttpPost("AddUserJobInfo")]
		public IActionResult AddUserJobInfo(UserJobInfoModel userJobInfo)
		{
			string sql = @$"
			INSERT INTO [TutorialAppSchema].[UserJobInfo] (
				[UserId],
				[JobTitle],
				[Department]
			) VALUES (
				'{userJobInfo.UserId}',
				'{userJobInfo.JobTitle}',
				'{userJobInfo.Department}'
			)";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Add User's Job Info.");
		}

		// Read - byId
		[HttpGet("GetUserJobInfo/{userId}")]
		public IEnumerable<UserJobInfoModel> GetUserJobInfo(int userId)
		{
			string sql = @$"
			SELECT
				[UserId],
				[JobTitle],
				[Department]
			FROM [TutorialAppSchema].[UserJobInfo]
			WHERE [UserId] = {userId}
			";

			IEnumerable<UserJobInfoModel> userJobsInfo = _dapper.LoadData<UserJobInfoModel>(sql);

			return userJobsInfo;
		}

		// Update
		[HttpPut("EditUserJobInfo")]
		public IActionResult EditUserJobInfo(UserJobInfoModel userJobInfo)
		{
			string sql = @$"
			UPDATE [TutorialAppSchema].[UserJobInfo] SET
				[JobTitle] = '{userJobInfo.JobTitle}',
				[Department] = '{userJobInfo.Department}'
			WHERE UserId = {userJobInfo.UserId}
			";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Update User's Job Info.");
		}

		// Delete
		[HttpDelete("DeleteUserJobInfo/{userId}")]
		public IActionResult DeleteUserJobInfo(int userId)
		{
			string sql = @$"
			DELETE
			FROM [TutorialAppSchema].[UserJobInfo]
			WHERE [UserId] = {userId}
			";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Delete User's Job Info.");
		}
	}
}
