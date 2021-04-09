#nullable disable

using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class TagUsageWithVotesContract
	{
		public TagUsageWithVotesContract(TagUsage usage, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory)
		{
			Count = usage.Count;
			Date = usage.Date;
			Id = usage.Id;
			Tag = new TagBaseContract(usage.Tag, languagePreference);

			Votes = usage.VotesBase.Select(v => new UserForApiContract(v.User, userIconFactory, UserOptionalFields.MainPicture)).ToArray();
		}

		public TagUsageWithVotesContract() { }

		public DateTime Date { get; init; }

		public int Count { get; init; }

		public long Id { get; init; }

		public TagBaseContract Tag { get; init; }

		public UserForApiContract[] Votes { get; init; }
	}
}
