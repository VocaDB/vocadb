#nullable disable

using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Ext;

public class EmbedSongViewModel
{
	private const int DefaultHeight = 315;
	private const int DefaultWidth = 560;

	private readonly PVHelper _pvHelper;

	public EmbedSongViewModel(PVHelper pvHelper)
	{
		_pvHelper = pvHelper;
	}

	public int ContainerHeight => PlayerHeight + 55;

	public PVContract CurrentPV { get; set; }

	public string CurrentServiceName => CurrentPV?.Service.ToString().ToLowerInvariant() ?? string.Empty;

	public int? Height { get; set; }

	public SongForApiContract Song { get; set; }

	public int? Width { get; set; }

	public PVContract[] MainPVs => _pvHelper.GetMainPVs(Song.PVs);

	public int PlayerHeight => Height ?? DefaultHeight;

	public int PlayerWidth => Width ?? DefaultWidth;
}