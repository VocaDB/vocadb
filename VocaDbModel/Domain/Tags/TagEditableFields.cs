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
		AliasedTo = 1,

		CategoryName = 2,

		Description = 4,

		HideFromSuggestions = 8,

		Names = 16,

		OriginalName = 32,

		Parent = 64,

		Picture = 128,

		RelatedTags = 256,

		Status = 512,

		Targets = 1024,

		WebLinks = 2048
	}
}
