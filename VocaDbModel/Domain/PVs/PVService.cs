#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Domain.PVs
{
	/// <summary>
	/// PV service identifier.
	/// </summary>
	/// <remarks>
	/// These values are supposed to be serialized as strings.
	/// </remarks>
	public enum PVService
	{
		NicoNicoDouga = 1 << 0,

		Youtube = 1 << 1,

		SoundCloud = 1 << 2,

		Vimeo = 1 << 3,

		Piapro = 1 << 4,

		Bilibili = 1 << 5,

		File = 1 << 6,

		LocalFile = 1 << 7,

		Creofuga = 1 << 8,

		Bandcamp = 1 << 9,
	}

	/// <summary>
	/// PV service flags.
	/// </summary>
	/// <remarks>
	/// These values must not change because they're saved as a bit array in DB.
	/// </remarks>
	[Flags]
	public enum PVServices
	{
		Nothing = 0,

		NicoNicoDouga = PVService.NicoNicoDouga,

		Youtube = PVService.Youtube,

		SoundCloud = PVService.SoundCloud,

		Vimeo = PVService.Vimeo,

		Piapro = PVService.Piapro,

		Bilibili = PVService.Bilibili,

		File = PVService.File,

		LocalFile = PVService.LocalFile,

		Creofuga = PVService.Creofuga,

		Bandcamp = PVService.Bandcamp,
	}

	public static class PVServicesExtensions
	{
		public static IEnumerable<PVService> ToIndividualSelections(this PVServices selections)
		{
			return EnumVal<PVServices>
				.GetIndividualValues(selections)
				.Select(s => (PVService)s);
		}
	}
}
