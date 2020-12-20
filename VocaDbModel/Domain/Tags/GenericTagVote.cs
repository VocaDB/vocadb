#nullable disable

using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Tags
{
	public abstract class GenericTagVote<TUsage> : TagVote where TUsage : TagUsage
	{
		private TUsage _tagUsage;

		public GenericTagVote() { }

		public GenericTagVote(TUsage usage, User user)
			: base(user)
		{
			Usage = usage;
		}

		public virtual TUsage Usage
		{
			get => _tagUsage;
			set
			{
				ParamIs.NotNull(() => value);
				_tagUsage = value;
			}
		}

		public override TagUsage UsageBase => Usage;
	}
}
