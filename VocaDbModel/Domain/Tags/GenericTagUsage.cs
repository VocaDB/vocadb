using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Tags
{
	public abstract class GenericTagUsage<TEntry, TVote> : TagUsage
		where TEntry : class, IEntryWithStatus
		where TVote : TagVote
	{
		private TEntry _entry;
		private IList<TVote> _votes = new List<TVote>();

#nullable disable
		public GenericTagUsage() { }
#nullable enable

		public GenericTagUsage(TEntry entry, Tag tag)
			: base(tag)
		{
			Entry = entry;
		}

		public virtual TEntry Entry
		{
			get => _entry;
			[MemberNotNull(nameof(_entry))]
			set
			{
				ParamIs.NotNull(() => value);
				_entry = value;
			}
		}

		public override IEntryWithStatus EntryBase => Entry;

		public virtual IList<TVote> Votes
		{
			get => _votes;
			set
			{
				ParamIs.NotNull(() => value);
				_votes = value;
			}
		}

		public override IEnumerable<TagVote> VotesBase => Votes;

		public override void Delete()
		{
			base.Delete();
			Votes.Clear();
		}

		public virtual TVote? FindVote(User? user)
		{
			return Votes.FirstOrDefault(v => v.User.Equals(user));
		}

		public override TagVote? RemoveVote(User? user)
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
