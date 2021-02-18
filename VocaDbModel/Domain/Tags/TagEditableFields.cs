#nullable disable

using System;

namespace VocaDb.Model.Domain.Tags
{
	/// <summary>
	/// Editable tag fields.
	/// Persisted in the DB as strings, so integer values can be changed, but strings cannot.
	/// </summary>
	[Flags]
	public enum TagEditableFields
	{
		Nothing = 0,

		[Obsolete]
		AliasedTo = 1 << 0,

		CategoryName = 1 << 1,

		Description = 1 << 2,

		HideFromSuggestions = 1 << 3,

		Names = 1 << 4,

		OriginalName = 1 << 5,

		Parent = 1 << 6,

		Picture = 1 << 7,

		RelatedTags = 1 << 8,

		Status = 1 << 9,

		Targets = 1 << 10,

		WebLinks = 1 << 11,
	}
}
