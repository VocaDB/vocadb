using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagUsageWithVotesContract : TagUsageContract {

		public TagUsageWithVotesContract(TagUsage usage) 
			: base(usage) {

			Votes = usage.VotesBase.Select(v => new UserContract(v.User)).ToArray();
		
		}

		public TagUsageWithVotesContract() {}

		public UserContract[] Votes { get; set; }

	}

}
