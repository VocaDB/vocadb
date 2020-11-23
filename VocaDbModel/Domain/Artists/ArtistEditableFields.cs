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

		Albums = 1,

		ArtistType = 2,

		BaseVoicebank = 4,

		Description = 8,

		Groups = 16,

		Names = 32,

		OriginalName = 64,

		Picture = 128,

		Pictures = 256,

		ReleaseDate = 512,

		Status = 1024,

		WebLinks = 2048

	}

}
