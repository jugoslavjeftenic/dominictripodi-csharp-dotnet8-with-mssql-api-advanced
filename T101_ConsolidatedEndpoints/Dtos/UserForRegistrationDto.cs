﻿namespace T101_ConsolidatedEndpoints.Dtos
{
	public class UserForRegistrationDto
	{
		public string FirstName { get; set; } = "";
		public string LastName { get; set; } = "";
		public string Email { get; set; } = "";
		public string Gender { get; set; } = "";
		public string Password { get; set; } = "";
		public string PasswordConfirm { get; set; } = "";
	}
}