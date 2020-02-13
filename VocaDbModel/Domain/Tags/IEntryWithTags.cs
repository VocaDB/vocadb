namespace VocaDb.Model.Domain.Tags {

	public interface IEntryWithTags : IEntryBase {

		bool AllowNotifications { get; }

		ITagManager Tags { get; }

	}

}
