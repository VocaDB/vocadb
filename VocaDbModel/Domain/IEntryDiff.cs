namespace VocaDb.Model.Domain {

	public interface IEntryDiff {

		string ChangedFieldsString { get; }

		bool IsSnapshot { get; }

	}

}
