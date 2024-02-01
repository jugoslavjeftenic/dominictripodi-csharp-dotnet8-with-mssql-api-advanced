namespace T101_ConsolidatedEndpoints.Dtos
{
	public class UserForLoginConfirmationDto
	{
		public byte[] PasswordHash { get; set; } = [];
		public byte[] PasswordSalt { get; set; } = [];
	}
}
