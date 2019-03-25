using System;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Domain.PVs {

	/// <summary>
	/// PV service identifier.
	/// </summary>
	/// <remarks>
	/// These values are supposed to be serialized as strings.
	/// </remarks>
	public enum PVService {

		NicoNicoDouga	= 1,

		Youtube			= 2,

		SoundCloud		= 4,

		Vimeo			= 8,

		Piapro			= 16,

		Bilibili		= 32,

		File			= 64,

		LocalFile		= 128,

		Creofuga		= 256,

		Bandcamp		= 512

	}

	/// <summary>
	/// PV service flags.
	/// </summary>
	/// <remarks>
	/// These values must not change because they're saved as a bit array in DB.
	/// </remarks>
	[Flags]
	public enum PVServices {

		Nothing			= 0,

		NicoNicoDouga	= PVService.NicoNicoDouga,

		Youtube			= PVService.Youtube,

		SoundCloud		= PVService.SoundCloud,

		Vimeo			= PVService.Vimeo,

		Piapro			= PVService.Piapro,

		Bilibili		= PVService.Bilibili,

		File			= PVService.File,

		LocalFile		= PVService.LocalFile,

		Creofuga        = PVService.Creofuga,

	}

	public static class PVServicesExtender {

		public static IEnumerable<PVService> ToIndividualSelections(this PVServices selections) {
			
			return EnumVal<PVServices>
				.GetIndividualValues(selections)
				.Select(s => (PVService)s);

		}

	}

}
