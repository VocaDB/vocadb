#nullable disable

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	/// <summary>
	/// Data contract for <see cref="UserMessage"/>.
	/// Email address is not included, just the URL to profile icon.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserMessageContract
	{
		public UserMessageContract() { }

		public UserMessageContract(UserMessage message, IUserIconFactory iconFactory, bool includeBody = false)
		{
			ParamIs.NotNull(() => message);

			Body = (includeBody ? message.Message : string.Empty);
			Created = message.Created;
			CreatedFormatted = Created.ToUniversalTime().ToString("g");
			HighPriority = message.HighPriority;
			Id = message.Id;
			Inbox = message.Inbox;
			Read = message.Read;
			Receiver = new UserForApiContract(message.Receiver, iconFactory, UserOptionalFields.MainPicture);
			Sender = (message.Sender != null ? new UserForApiContract(message.Sender, iconFactory, UserOptionalFields.MainPicture) : null);
			Subject = message.Subject;
		}

		[DataMember]
		public string Body { get; set; }

		// Currently unable to parse raw datetime on client side, therefore only sending formatted datetime instead.
		public DateTime Created { get; set; }

		[DataMember]
		public string CreatedFormatted { get; set; }

		[DataMember]
		public bool HighPriority { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public UserInboxType Inbox { get; set; }

		[DataMember]
		public bool Read { get; set; }

		[DataMember]
		public UserForApiContract Receiver { get; set; }

		[DataMember]
		public UserForApiContract Sender { get; set; }

		[DataMember]
		public string Subject { get; set; }

		public override string ToString()
		{
			return $"Message '{Subject}' to {Receiver} [{Id}]";
		}
	}
}
