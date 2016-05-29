using System;
using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagUsageWithVotesContract {

		public TagUsageWithVotesContract(TagUsage usage, ContentLanguagePreference languagePreference) {

			Count = usage.Count;
			Date = usage.Date;
			Id = usage.Id;
			Tag = new TagBaseContract(usage.Tag, languagePreference);

			Votes = usage.VotesBase.Select(v => new UserContract(v.User)).ToArray();

		}

		public TagUsageWithVotesContract() {}

		public DateTime Date { get; set; }

		public int Count { get; set; }

		public long Id { get; set; }

		public TagBaseContract Tag { get; set; }

		public UserContract[] Votes { get; set; }

	}

}
