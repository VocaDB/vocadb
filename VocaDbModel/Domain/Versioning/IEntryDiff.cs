namespace VocaDb.Model.Domain.Versioning {

	public interface IEntryDiff {

		//string[] ChangedFieldNames { get; }

		string ChangedFieldsString { get; }

		bool IsSnapshot { get; }

	}

}
