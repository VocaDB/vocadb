using System;

namespace VocaDb.Model.Domain.Versioning {

	public interface IArchivedObjectVersionWithFields<TField> where TField : struct, IConvertible {

		/// <summary>
		/// Checks whether a specific field is included in this diff.
		/// </summary>
		/// <param name="field">Field to be checked.</param>
		/// <returns>True if the field is included, otherwise false.</returns>
		/// <remarks>
		/// Snapshots include all fields except the Cover.
		/// Other fields are commonly included only they are changed.
		/// </remarks>
		bool IsIncluded(TField field);

	}

}
