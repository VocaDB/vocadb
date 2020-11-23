namespace VocaDb.Model.Domain.Versioning
{

	public interface IEntryDiff
	{

		string[] ChangedFieldNames { get; }

		/// <summary>
		/// Comma-separated list of the names of the changed fields.
		/// For example "Artists,MainPicture,Names".
		/// </summary>
		string ChangedFieldsString { get; }

		bool IsSnapshot { get; }

	}

}
