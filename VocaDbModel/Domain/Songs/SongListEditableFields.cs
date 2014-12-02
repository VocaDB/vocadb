using System;

namespace VocaDb.Model.Domain.Songs {

	[Flags]
	public enum SongListEditableFields {

		Nothing				= 0,

		Description			= 1,

		FeaturedCategory	= 2,

		Name				= 4,

		Songs				= 8,

		Thumbnail			= 16

	}

}
