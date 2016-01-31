using System;

namespace VocaDb.Model.Domain.Tags {

	/// <summary>
	/// Editable tag fields.
	/// Persisted in the DB as strings, so integer values can be changed, but strings cannot.
	/// </summary>
	[Flags]
	public enum TagEditableFields {

		Nothing			= 0,

		AliasedTo		= 1,

		CategoryName	= 2,

		Description		= 4,

		Names			= 8,

		OriginalName	= 16,

		Parent			= 32,

		Picture			= 64,

		RelatedTags		= 128,

		Status			= 256

	}

}
