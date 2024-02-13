using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using T101_ConsolidatedEndpoints.Data;
using T101_ConsolidatedEndpoints.Models;

namespace T101_ConsolidatedEndpoints.Helpers
{
	public class ReusableSql(IConfiguration config)
	{
		private readonly DataContextDapper _dapper = new(config);

		public bool UpsertUser(UserModel user)
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
			sqlParameters.Add("@FirstNameParam", user.FirstName, DbType.String);
			sqlParameters.Add("@LastNameParam", user.LastName, DbType.String);
			sqlParameters.Add("@EmailParam", user.Email, DbType.String);
			sqlParameters.Add("@GenderParam", user.Gender, DbType.String);
			sqlParameters.Add("@ActiveParam", user.Active, DbType.Boolean);
			sqlParameters.Add("@JobTitleParam", user.JobTitle, DbType.String);
			sqlParameters.Add("@DepartmentParam", user.Department, DbType.String);
			sqlParameters.Add("@SalaryParam", user.Salary, DbType.Decimal);
			sqlParameters.Add("@UserIdParam", user.UserId, DbType.Int32);

			return _dapper.ExecuteSqlWithParameters(sql, sqlParameters);
		}
	}
}
