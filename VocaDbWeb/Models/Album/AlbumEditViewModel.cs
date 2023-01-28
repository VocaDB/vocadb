#nullable disable

using Microsoft.AspNetCore.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Utils;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Album;

public class AlbumEditViewModel
{
	public AlbumEditViewModel()
	{
		AllDiscTypes = Translate.DiscTypeNames.GetValuesAndNamesStrings(AppConfig.AlbumTypes);

		DiscTypeDescriptions = ViewRes.Album.EditStrings.BaDiscTypeExplanation
							   + "<br /><br /><ul>" + string.Join("",
								   EnumVal<DiscType>.Values.Where(v => v != DiscType.Unknown).Select(v => $"<li><strong>{Translate.DiscTypeName(v)}</strong>: {(global::Resources.DiscTypeDescriptions.ResourceManager.GetString(v.ToString()))}</li>"));
	}

	public AlbumEditViewModel(AlbumContract album, IUserPermissionContext permissionContext,
		bool canDelete,
		AlbumForEditContract editedAlbum = null)
		: this()
	{
		ParamIs.NotNull(() => album);

		Album = album;
		CanDelete = canDelete;
		EditedAlbum = editedAlbum;

		AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(permissionContext).ToArray();
	}

	public AlbumContract Album { get; set; }

	public Dictionary<string, string> AllDiscTypes { get; set; }

	public EntryStatus[] AllowedEntryStatuses { get; set; }

	public bool CanDelete { get; set; }

	public string DiscTypeDescriptions { get; set; }

	public bool Draft => Album != null && Album.Status == EntryStatus.Draft;

	[ModelBinder(BinderType = typeof(JsonModelBinder))]
	// TODO: implement [AllowHtml]
	public AlbumForEditContract EditedAlbum { get; set; }

	public bool HasCoverPicture => Album != null && !string.IsNullOrEmpty(Album.CoverPictureMime);

	public int Id => Album != null ? Album.Id : 0;

	public string Name => Album != null ? Album.Name : null;

	public void CheckModel()
	{
		if (EditedAlbum == null)
			throw new InvalidFormException("Model was null");

		if (EditedAlbum.ArtistLinks == null)
			throw new InvalidFormException("Artists list was null");

		if (EditedAlbum.Identifiers == null)
			throw new InvalidFormException("Identifiers list was null");

		if (EditedAlbum.Names == null)
			throw new InvalidFormException("Names list was null");

		if (EditedAlbum.PVs == null)
			throw new InvalidFormException("PVs list was null");

		if (EditedAlbum.Songs == null)
			throw new InvalidFormException("Tracks list was null");

		if (EditedAlbum.WebLinks == null)
			throw new InvalidFormException("WebLinks list was null");
	}
}