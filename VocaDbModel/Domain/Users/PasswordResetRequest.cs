using System;

namespace VocaDb.Model.Domain.Users
{
	public class PasswordResetRequest : IDatabaseObject
	{
		public static readonly TimeSpan ExpirationTime = TimeSpan.FromDays(1);

		private User user;

		public PasswordResetRequest()
		{
			Created = DateTime.Now;
			Email = string.Empty;
		}

		public PasswordResetRequest(User user)
			: this()
		{
			User = user;
			Email = user.Email;
		}

		public virtual DateTime Created { get; set; }

		/// <summary>
		/// Email to which this request was sent. Might be different from user's current email.
		/// </summary>
		public virtual string Email { get; set; }

		public virtual Guid Id { get; set; }

		public virtual bool IsValid => Created >= DateTime.Now - ExpirationTime;

		public virtual User User
		{
			get => user;
			set
			{
				ParamIs.NotNull(() => value);
				user = value;
			}
		}
	}
}
