using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using T101_ConsolidatedEndpoints.Data;
using T101_ConsolidatedEndpoints.Models;

namespace T101_ConsolidatedEndpoints.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class PostsController(IConfiguration config) : ControllerBase
	{
		private readonly DataContextDapper _dapper = new(config);

		// Get
		[HttpGet("{postId}/{userId}/{searchParam}")]
		public IEnumerable<PostModel> GetPost(int postId = 0, int userId = 0, string searchParam = "none")
		{
			string sql = "EXEC TutorialAppSchema.spPosts_Get";
			string stringParameters = "";
			DynamicParameters sqlParameters = new();

			if (postId != 0)
			{
				stringParameters += ", @PostId = @PostIdParam";
				sqlParameters.Add("@PostIdParam", postId, DbType.Int32);
			}

			if (userId != 0)
			{
				stringParameters += ", @UserId = @UserIdParam";
				sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
			}

			if (searchParam != "none")
			{
				stringParameters += ", @SearchValue = @SearchValueParam";
				sqlParameters.Add("@SearchValueParam", searchParam, DbType.String);
			}

			if (stringParameters.Length > 0)
			{
				sql += stringParameters[1..];
			}

			IEnumerable<PostModel> posts = _dapper.LoadDataWithParameters<PostModel>(sql, sqlParameters);

			return posts;
		}

		// Get - byLoggedUser
		[HttpGet]
		public IEnumerable<PostModel> GetPostByLoggedUser()
		{
			string sql = $"EXEC TutorialAppSchema.spPosts_Get @UserId = @UserIdParam";
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@UserIdParam", User.FindFirst("userId")?.Value, DbType.Int32);

			IEnumerable<PostModel> posts = _dapper.LoadDataWithParameters<PostModel>(sql, sqlParameters);

			return posts;
		}

		// Upsert
		[HttpPut]
		public IActionResult UpsertPost(PostModel post)
		{
			string sql = @$"
			EXEC TutorialAppSchema.spPosts_Upsert
				@UserId = @UserIdParam,
				@PostTitle = @PostTitleParam,
				@PostContent = @PostContentParam
			";
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@UserIdParam", User.FindFirst("userId")?.Value, DbType.Int32);
			sqlParameters.Add("@PostTitleParam", post.PostTitle, DbType.String);
			sqlParameters.Add("@PostContentParam", post.PostContent, DbType.String);

			if (post.PostId > 0)
			{
				sql += ", @PostId = @PostIdParam";
				sqlParameters.Add("@PostIdParam", post.PostId, DbType.Int32);
			}

			if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
			{
				return Ok();
			}

			return StatusCode(400, "Failed to Upsert Post.");
		}

		// Delete
		[HttpDelete("{postId}")]
		public IActionResult DeletePost(int postId)
		{
			string sql = @$"
			EXEC TutorialAppSchema.spPost_Delete
				@PostId = @PostIdParam,
				@UserId = @UserIdParam
			";
			DynamicParameters sqlParameters = new();
			sqlParameters.Add("@PostIdParam", postId, DbType.Int32);
			sqlParameters.Add("@UserIdParam", User.FindFirst("userId")?.Value, DbType.Int32);

			if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
			{
				return Ok();
			}

			return StatusCode(400, "Failed to Delete Post.");
		}
	}
}
