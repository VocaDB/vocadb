using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Tags;

public interface IEntryWithTags : IEntryBase
{
	/// <summary>
	/// If this is set to false, notifications will not be sent to users.
	/// </summary>
	bool AllowNotifications { get; }

	public Object TagSubtype();

	public string TagTarget()
	{
		var entryType = EntryType;
		if (entryType == EntryType.ReleaseEventSeries) entryType = EntryType.ReleaseEvent;
		if (entryType == EntryType.Artist && ArtistHelper.VoiceSynthesizerTypes.Contains((ArtistType) TagSubtype()))
		{
			return $"voicesynthesizer:{TagSubtype().ToString().ToLower()}";
		}
		return $"{entryType.ToString().ToLower()}:{TagSubtype().ToString().ToLower()}";
	}

	ITagManager Tags { get; }
}
