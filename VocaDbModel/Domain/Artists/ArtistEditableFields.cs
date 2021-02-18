#nullable disable

using System;

namespace VocaDb.Model.Domain.Artists
{
	/// <summary>
	/// Saved in the database as strings - numbers can be changed, but not the names.
	/// </summary>
	[Flags]
	public enum ArtistEditableFields
	{
		Nothing = 0,

		Albums = 1 << 0,

		ArtistType = 1 << 1,

		BaseVoicebank = 1 << 2,

		Description = 1 << 3,

		Groups = 1 << 4,

		Names = 1 << 5,

		OriginalName = 1 << 6,

		Picture = 1 << 7,

		Pictures = 1 << 8,

		ReleaseDate = 1 << 9,

		Status = 1 << 10,

		WebLinks = 1 << 11,
	}
}
