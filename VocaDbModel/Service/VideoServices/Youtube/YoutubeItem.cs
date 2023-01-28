#nullable disable


namespace VocaDb.Model.Service.VideoServices.Youtube;

public interface IYoutubeItem { }

public class YoutubeItem<TSnippet> : IYoutubeItem where TSnippet : Snippet
{
	public TSnippet Snippet { get; set; }
}
