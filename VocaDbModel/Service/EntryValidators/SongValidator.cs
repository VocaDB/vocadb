using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Resources;

namespace VocaDb.Model.Service.EntryValidators {

	public class SongValidator {

		public bool IsValid(Song song, int instrumentalTagId) {
			
			ParamIs.NotNull(() => song);

			var errors = new List<string>();

			if (song.SongType == SongType.Unspecified)
				errors.Add(SongValidationErrors.NeedType);

			var derivedTypes = new[] { SongType.Remaster, SongType.Cover, SongType.Instrumental, SongType.MusicPV, SongType.Other, SongType.Remix };

			if (song.Notes.IsEmpty && !song.HasOriginalVersion && derivedTypes.Contains(song.SongType)) {
				errors.Add(SongValidationErrors.NeedOriginal);			
			}

			if (song.Artists.All(a => a.Artist == null))
				errors.Add(SongValidationErrors.NeedArtist);

			if (song.Names.Names.All(n => n.Language == ContentLanguageSelection.Unspecified))
				errors.Add(SongValidationErrors.UnspecifiedNames);

			if (song.SongType != SongType.Instrumental 
				&& song.SongType != SongType.DramaPV 
				&& !song.Tags.HasTag(instrumentalTagId) 
				&& !ArtistHelper.GetVocalists(song.Artists.ToArray()).Any())
				errors.Add(SongValidationErrors.NonInstrumentalSongNeedsVocalists);

			if (!song.Artists.Any(a => a.Artist != null && ArtistHelper.IsProducerRole(a, SongHelper.GetContentFocus(song.SongType))))
				errors.Add(SongValidationErrors.NeedProducer);

			if (song.Artists.GroupBy(a => (a.Artist != null ? a.Artist.Id.ToString() : a.Name) + a.IsSupport).Any(a => a.Count() > 1))
				errors.Add(SongValidationErrors.DuplicateArtist);

			return !errors.Any();

		}

	}

}
