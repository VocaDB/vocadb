#nullable disable

using System;
using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class TagUsageWithVotesContract
	{
		public TagUsageWithVotesContract(TagUsage usage, ContentLanguagePreference languagePreference)
		{
			Count = usage.Count;
			Date = usage.Date;
			Id = usage.Id;
			Tag = new TagBaseContract(usage.Tag, languagePreference);

			Votes = usage.VotesBase.Select(v => new UserContract(v.User)).ToArray();
		}

		public TagUsageWithVotesContract() { }

		public DateTime Date { get; init; }

		public int Count { get; init; }

		public long Id { get; init; }

		public TagBaseContract Tag { get; init; }

		public UserContract[] Votes { get; init; }
	}
}
