namespace T101_ConsolidatedEndpoints.Dtos
{
	public class PostToEditDto
	{
		public int PostId { get; set; }
		public string PostTitle { get; set; } = "";
		public string PostContent { get; set; } = "";
	}
}
