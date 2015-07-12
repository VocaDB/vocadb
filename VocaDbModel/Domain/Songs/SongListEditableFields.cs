using System;

namespace VocaDb.Model.Domain.Songs {

	[Flags]
	public enum SongListEditableFields {

		Nothing				= 0,

		Description			= 1,

		EventDate			= 2,

		FeaturedCategory	= 4,

		Name				= 8,

		Songs				= 16,

		Status				= 32,

		Thumbnail			= 64

	}

}
