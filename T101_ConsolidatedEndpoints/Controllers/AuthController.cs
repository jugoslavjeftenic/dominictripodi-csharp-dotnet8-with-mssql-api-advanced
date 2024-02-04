using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using T101_ConsolidatedEndpoints.Data;
using T101_ConsolidatedEndpoints.Dtos;
using T101_ConsolidatedEndpoints.Helpers;
using T101_ConsolidatedEndpoints.Models;

namespace T101_ConsolidatedEndpoints.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class AuthController(IConfiguration config) : ControllerBase
	{
		private readonly DataContextDapper _dapper = new(config);
		private readonly AuthHelper _authHelper = new(config);

		[AllowAnonymous]
		[HttpPost("Register")]
		public IActionResult Register(UserForRegistrationDto userForRegistration)
		{
			if (userForRegistration.Password == userForRegistration.PasswordConfirm)
			{
				string sqlCheckUserExists =
					$"SELECT [Email] FROM TutorialAppSchema.Auth WHERE Email = '{userForRegistration.Email}'";

				IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

				if (!existingUsers.Any())
				{
					UserForLoginDto userForSetPassword = new()
					{
						Email = userForRegistration.Email,
						Password = userForRegistration.Password
					};

					if (_authHelper.SetPassword(userForSetPassword))
					{
						string _specifier = "0.00";
						CultureInfo _culture = CultureInfo.InvariantCulture;

						string sqlAddUser = @$"
						EXEC TutorialAppSchema.spUser_Upsert
							@FirstName = '{userForRegistration.FirstName}',
							@LastName = '{userForRegistration.LastName}',
							@Email = '{userForRegistration.Email}',
							@Gender = '{userForRegistration.Gender}',
							@Active = 1,
							@JobTitle = '{userForRegistration.JobTitle}',
							@Department = '{userForRegistration.Department}',
							@Salary = {userForRegistration.Salary.ToString(_specifier, _culture)}
						";

						if (_dapper.ExecuteSql(sqlAddUser))
						{
							return Ok();
						}

						return StatusCode(400, "Failed to add User.");
					}

					return StatusCode(400, "Failed to register User.");
				}

				return StatusCode(400, "User with this email already exists!");
			}

			return StatusCode(400, "Passwords do not match!");
		}

		[HttpPost("ResetPassword")]
		public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
		{
			if (_authHelper.SetPassword(userForSetPassword))
			{
				return Ok();
			}

			return StatusCode(400, "Failed to update password.");
		}

		[AllowAnonymous]
		[HttpPost("Login")]
		public IActionResult Login(UserForLoginDto userForLogin)
		{
			string sqlForHashAndSalt = @$"
			SELECT 
				[PasswordHash], 
				[PasswordSalt] 
			FROM TutorialAppSchema.Auth 
			WHERE Email = '{userForLogin.Email}'
			";

			UserForLoginConfirmationDto userForLoginConfirmation =
				_dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);


			byte[] passwordHash =
				_authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

			for (int i = 0; i < passwordHash.Length; i++)
			{
				if (passwordHash[i] != userForLoginConfirmation.PasswordHash[i])
				{
					return StatusCode(401, "Incorrect password!");
				}
			}

			string sqlUserId = @$"
			SELECT
				[UserId]
			FROM TutorialAppSchema.Users 
			WHERE Email = '{userForLogin.Email}'
			";

			int userId = _dapper.LoadDataSingle<int>(sqlUserId);

			return Ok(new Dictionary<string, string> { { "token", _authHelper.CreateToken(userId) } });
		}

		[HttpGet("RefreshToken")]
		public string RefreshToken()
		{
			string sqlUserId = @$"
			SELECT
				[UserId]
			FROM TutorialAppSchema.Users 
			WHERE UserId = '{User.FindFirst("userId")?.Value}'
			";

			int userId = _dapper.LoadDataSingle<int>(sqlUserId);

			return _authHelper.CreateToken(userId);
		}
	}
}
