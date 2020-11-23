using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Tags
{
	public abstract class GenericTagUsage<TEntry, TVote> : TagUsage where TEntry : class, IEntryBase where TVote : TagVote
	{
		private TEntry entry;
		private IList<TVote> votes = new List<TVote>();

		public GenericTagUsage() { }

		public GenericTagUsage(TEntry entry, Tag tag)
			: base(tag)
		{
			Entry = entry;
		}

		public virtual TEntry Entry
		{
			get => entry;
			set
			{
				ParamIs.NotNull(() => value);
				entry = value;
			}
		}

		public override IEntryBase EntryBase => Entry;

		public virtual IList<TVote> Votes
		{
			get => votes;
			set
			{
				ParamIs.NotNull(() => value);
				votes = value;
			}
		}

		public override IEnumerable<TagVote> VotesBase => Votes;

		public override void Delete()
		{
			base.Delete();
			Votes.Clear();
		}

		public virtual TVote FindVote(User user)
		{
			return Votes.FirstOrDefault(v => v.User.Equals(user));
		}

		public override TagVote RemoveVote(User user)
		{
			var vote = FindVote(user);

			if (vote == null)
				return null;

			Votes.Remove(vote);
			Count--;

			return vote;
		}
	}
}
