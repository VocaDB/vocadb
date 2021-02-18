#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Domain.Songs
{
	public enum SongType
	{
		Unspecified = 0,

		Original = 1 << 0,

		Remaster = 1 << 1,

		Remix = 1 << 2,

		Cover = 1 << 3,

		Arrangement = 1 << 4,

		Instrumental = 1 << 5,

		Mashup = 1 << 6,

		MusicPV = 1 << 7,

		DramaPV = 1 << 8,

		Live = 1 << 9,

		Illustration = 1 << 10,

		Other = 1 << 11,
	}

	[Flags]
	public enum SongTypes
	{
		Unspecified = 0,

		Original = 1 << 0,

		Remaster = 1 << 1,

		Remix = 1 << 2,

		Cover = 1 << 3,

		Instrumental = 1 << 4,

		Mashup = 1 << 5,

		MusicPV = 1 << 6,

		DramaPV = 1 << 7,

		Live = 1 << 8,

		Illustration = 1 << 9,

		Other = 1 << 10,
	}

	public static class SongTypesExtensions
	{
		public static IEnumerable<SongType> ToIndividualSelections(this SongTypes selections, bool skipUnspecified = false)
		{
			return EnumVal<SongTypes>
				.GetIndividualValues(selections)
				.Where(t => !skipUnspecified || t != SongTypes.Unspecified)
				.Select(s => (SongType)s);
		}
	}
}
