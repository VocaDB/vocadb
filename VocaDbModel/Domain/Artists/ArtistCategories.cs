using System;

namespace VocaDb.Model.Domain.Artists {

	[Flags]
	public enum ArtistCategories {

		Nothing		= 0,

		Vocalist	= 1,

		Producer	= 2,

		Animator	= 4,

		Label		= 8,

		Circle		= 16,

		Other		= 32,

		Band		= 64

	}

}
