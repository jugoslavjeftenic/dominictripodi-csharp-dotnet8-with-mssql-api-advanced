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

		// PostModel -------------------------------------------
		// Create
		[HttpPost("AddPost")]
		public IActionResult AddPost(PostToAddDto post)
		{
			string sql = @$"
			INSERT INTO [TutorialAppSchema].[Posts] (
				[UserId],
				[PostTitle],
				[PostContent],
				[PostCreated],
				[PostUpdated]
			) VALUES (
				{User.FindFirst("userId")?.Value},
				'{post.PostTitle}',
				'{post.PostContent}',
				GETDATE(),
				GETDATE()
			)";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Add Post");
		}

		// Read - all
		[HttpGet("GetPosts")]
		public IEnumerable<PostModel> GetPosts()
		{
			string sql = @"
			SELECT
				[PostId],
				[UserId],
				[PostTitle],
				[PostContent],
				[PostCreated],
				[PostUpdated]
			FROM [TutorialAppSchema].[Posts]
			";

			return _dapper.LoadData<PostModel>(sql);
		}

		// Read - byPostId
		[HttpGet("GetPost/{postId}")]
		public PostModel GetPost(int postId)
		{
			string sql = @$"
			SELECT
				[PostId],
				[UserId],
				[PostTitle],
				[PostContent],
				[PostCreated],
				[PostUpdated]
			FROM [TutorialAppSchema].[Posts]
			WHERE [PostId] = {postId}
			";

			return _dapper.LoadDataSingle<PostModel>(sql);
		}

		// Read - byUserId
		[HttpGet("GetPostsByUser/{userId}")]
		public IEnumerable<PostModel> GetPostsByUser(int userId)
		{
			string sql = @$"
			SELECT
				[PostId],
				[UserId],
				[PostTitle],
				[PostContent],
				[PostCreated],
				[PostUpdated]
			FROM [TutorialAppSchema].[Posts]
			WHERE [UserId] = {userId}
			";

			return _dapper.LoadData<PostModel>(sql);
		}

		// Read - byMyUserId
		[HttpGet("GetPostsByMyUser")]
		public IEnumerable<PostModel> GetPostsByMyUser()
		{
			string sql = @$"
			SELECT
				[PostId],
				[UserId],
				[PostTitle],
				[PostContent],
				[PostCreated],
				[PostUpdated]
			FROM [TutorialAppSchema].[Posts]
			WHERE [UserId] = {User.FindFirst("userId")?.Value}
			";

			return _dapper.LoadData<PostModel>(sql);
		}

		// Read - bySearch
		[HttpGet("GetPostsBySearch/{searchParam}")]
		public IEnumerable<PostModel> GetPostsBySearch(string searchParam)
		{
			string sql = @$"
			SELECT
				[PostId],
				[UserId],
				[PostTitle],
				[PostContent],
				[PostCreated],
				[PostUpdated]
			FROM [TutorialAppSchema].[Posts]
			WHERE [PostTitle] LIKE '%{searchParam}%' OR [PostContent] LIKE '%{searchParam}%'
			";

			return _dapper.LoadData<PostModel>(sql);
		}

		// Update
		[HttpPut("EditPost")]
		public IActionResult EditPost(PostToEditDto post)
		{
			string sql = @$"
			UPDATE [TutorialAppSchema].[Posts] SET
				[PostTitle] = '{post.PostTitle}',
				[PostContent] = '{post.PostContent}',
				[PostUpdated] = GETDATE()
			WHERE PostId = {post.PostId} AND UserId = {User.FindFirst("userId")?.Value}
			";

			if (_dapper.ExecuteSql(sql))
			{
				return Ok();
			}

			throw new Exception("Failed to Update Post");
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
