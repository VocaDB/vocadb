using System;
using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models.User
{

	public class ResetPassword
	{

		[Required]
		[Display(Name = "New password")]
		[DataType(DataType.Password)]
		[Compare("NewPassAgain", ErrorMessage = "Passwords must match")]
		[StringLength(100, MinimumLength = 5)]
		public string NewPass { get; set; }

		[Required]
		[Display(Name = "New password again")]
		[DataType(DataType.Password)]
		[StringLength(100, MinimumLength = 5)]
		public string NewPassAgain { get; set; }

		public Guid RequestId { get; set; }

	}

}