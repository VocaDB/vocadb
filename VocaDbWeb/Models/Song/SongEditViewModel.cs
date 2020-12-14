#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Utils;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Song
{
	[PropertyModelBinder]
	public class SongEditViewModel
	{
		public SongEditViewModel()
		{
			AllPVTypes = EnumVal<PVType>.Values;
			AllVideoServices = EnumVal<PVService>.Values;
		}

		public SongEditViewModel(SongContract song, IUserPermissionContext permissionContext,
			bool canDelete,
			int instrumentalTagId,
			SongForEditContract editedSong = null, int? albumId = null)
			: this()
		{
			ParamIs.NotNull(() => song);

			Song = song;
			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(permissionContext).ToArray();
			CanDelete = canDelete;
			InstrumentalTagId = instrumentalTagId;
			EditedSong = editedSong;
			AlbumId = albumId;
		}

		public int? AlbumId { get; set; }

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		public PVType[] AllPVTypes { get; set; }

		public Dictionary<string, string> AllSongTypes
		{
			get
			{
				return Translate.SongTypeNames.GetValuesAndNamesStrings(AppConfig.SongTypes);
			}
		}

		public PVService[] AllVideoServices { get; set; }

		public bool CanDelete { get; set; }

		public bool Draft
		{
			get { return Song != null && Song.Status == EntryStatus.Draft; }
		}

		[FromJson]
		[AllowHtml]
		public SongForEditContract EditedSong { get; set; }

		public int Id
		{
			get { return Song != null ? Song.Id : 0; }
		}

		public int InstrumentalTagId { get; set; }

		public string Name
		{
			get { return Song != null ? Song.Name : null; }
		}

		public SongContract Song { get; set; }

		public void CheckModel()
		{
			if (EditedSong == null)
				throw new InvalidFormException("Model was null");

			if (EditedSong.Artists == null)
				throw new InvalidFormException("ArtistLinks list was null"); // Shouldn't be null

			if (EditedSong.Lyrics == null)
				throw new InvalidFormException("Lyrics list was null"); // Shouldn't be null

			if (EditedSong.Names == null)
				throw new InvalidFormException("Names list was null"); // Shouldn't be null

			if (EditedSong.PVs == null)
				throw new InvalidFormException("PVs list was null"); // Shouldn't be null

			if (EditedSong.WebLinks == null)
				throw new InvalidFormException("WebLinks list was null"); // Shouldn't be null
		}
	}
}