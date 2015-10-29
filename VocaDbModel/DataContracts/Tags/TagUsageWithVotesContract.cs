using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagUsageWithVotesContract {

		public TagUsageWithVotesContract(TagUsage usage) {

			Count = usage.Count;
			Id = usage.Id;
			Tag = new TagBaseContract(usage.Tag);

			Votes = usage.VotesBase.Select(v => new UserContract(v.User)).ToArray();

		}

		public TagUsageWithVotesContract() {}

		public UserContract[] Votes { get; set; }

		public int Count { get; set; }

		public long Id { get; set; }

		public TagBaseContract Tag { get; set; }

	}

}
