using System;

namespace VocaDb.Model.Domain.Users
{

	/// <summary>
	/// Message sent to a user. 
	/// Can be either a personal message from one user to another, or a notification.
	/// </summary>
	public class UserMessage : IEntryWithIntId
	{

		public static UserMessage CreateReceived(User from, User to, string subject, string body, bool highPriority)
		{

			return new UserMessage(to, UserInboxType.Received, from, to, subject, body, highPriority);

		}

		public static UserMessage CreateSent(User from, User to, string subject, string body, bool highPriority)
		{

			return new UserMessage(from, UserInboxType.Sent, from, to, subject, body, highPriority);

		}

		private string message;
		private User receiver;
		private string subject;

		public UserMessage()
		{
			Created = DateTime.Now;
		}

		/// <summary>
		/// Creates a new notification message (no sender).
		/// </summary>
		public UserMessage(User to, string subject, string body, bool highPriority)
			: this()
		{

			ParamIs.NotNull(() => to);

			User = to;
			Receiver = to;
			Subject = subject;
			Message = body;
			HighPriority = highPriority;

			Inbox = UserInboxType.Notifications;

		}

		public UserMessage(User user, UserInboxType inbox, User from, User to, string subject, string body, bool highPriority)
			: this()
		{

			ParamIs.NotNull(() => user);

			User = user;
			Inbox = inbox;
			Sender = from;
			Receiver = to;
			Subject = subject;
			Message = body;
			HighPriority = highPriority;

		}

		/// <summary>
		/// Timestamp when message was created (sent).
		/// </summary>
		public virtual DateTime Created { get; set; }

		public virtual bool HighPriority { get; set; }

		public virtual int Id { get; set; }

		/// <summary>
		/// Inbox in which the message was sent.
		/// </summary>
		/// <remarks>
		/// When a message is sent from one user to another, it is saved in sender's <see cref="UserInboxType.Sent"/> inbox 
		/// and receivers <see cref="UserInboxType.Received"/> inbox.
		/// </remarks>
		public virtual UserInboxType Inbox { get; set; }

		/// <summary>
		/// Message body. May contain Markdown markup.
		/// Cannot be null or empty.
		/// </summary>
		public virtual string Message
		{
			get => message;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				message = value;
			}
		}

		public virtual bool Read { get; set; }

		/// <summary>
		/// Receiver of this message. Cannot be null.
		/// </summary>
		public virtual User Receiver
		{
			get => receiver;
			set
			{
				ParamIs.NotNull(() => value);
				receiver = value;
			}
		}

		/// <summary>
		/// Sender of this message. Can be null, in which case it's a notification.
		/// </summary>
		public virtual User Sender { get; set; }

		public virtual string Subject
		{
			get => subject;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				subject = value;
			}
		}

		/// <summary>
		/// User in whose inbox this message is.
		/// Cannot be null.
		/// </summary>
		public virtual User User { get; set; }

		public override string ToString()
		{
			// Note: no message contents in ToString because personal information might be logged
			return string.Format("User message [{0}]", Id);
		}

	}

	public enum UserInboxType
	{
		Nothing,
		Received,
		Sent,
		Notifications
	}

}
