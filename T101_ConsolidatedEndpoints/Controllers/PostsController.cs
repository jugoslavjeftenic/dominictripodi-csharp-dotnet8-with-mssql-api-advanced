using Microsoft.AspNetCore.Mvc;
using T101_ConsolidatedEndpoints.Data;
using T101_ConsolidatedEndpoints.Dtos;
using T101_ConsolidatedEndpoints.Models;

namespace T101_ConsolidatedEndpoints.Controllers
{
	//[Authorize]
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
			string parameters = "";

			if (postId != 0)
			{
				parameters += $", @PostId={postId}";
			}

			if (userId != 0)
			{
				parameters += $", @UserId={userId}";
			}

			if (searchParam != "none")
			{
				parameters += $", @SearchValue='{searchParam}'";
			}

			if (parameters.Length > 0)
			{
				sql += parameters[1..];
			}

			return _dapper.LoadData<PostModel>(sql);
		}

		// Get - byLoggedUser
		[HttpGet]
		public IEnumerable<PostModel> GetPostByLoggedUser()
		{
			string sql = @$"
			EXEC TutorialAppSchema.spPosts_Get
			WHERE [UserId] = {User.FindFirst("userId")?.Value}
			";

			return _dapper.LoadData<PostModel>(sql);
		}

		// Upsert
		[HttpPut]
		public IActionResult UpsertPost(PostModel post)
		{
			string sql = @$"
			EXEC TutorialAppSchema.spPosts_Upsert
				@UserId = {User.FindFirst("userId")?.Value},
				@PostTitle = '{post.PostTitle}',
				@PostContent = '{post.PostContent}'
			";

			if (post.PostId > 0)
			{
				sql += $", @PostId = {post.PostId}";
			}

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			return StatusCode(400, "Failed to Upsert Post.");
		}

		// Delete
		[HttpDelete("DeletePost/{postId}")]
		public IActionResult DeleteUser(int postId)
		{
			string sql = @$"
			DELETE
			FROM [TutorialAppSchema].[Posts]
			WHERE [PostId] = {postId} AND UserId = {User.FindFirst("userId")?.Value}
			";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Delete Post");
		}
	}
}
