namespace VocaDb.Model.Service.VideoServices.PiaproClient;

using System;

/// <summary>
/// Result of a song post parsing.
/// </summary>
public class PostQueryResult
{
	/// <summary>
	/// URL to artwork, if any.
	/// For example "https://cdn.piapro.jp/thumb_i/5i/5i20uhj17deukmea_20110212213034_0500_0500.jpg"
	/// Note: for generic/missing artwork, this will be empty.
	/// Cannot be null. Can be empty if there is no artwork.
	/// </summary>
	public string ArtworkUrl { get; set; }

	/// <summary>
	/// Author's nickname (ニックネーム), for example "ハチ".
	/// Cannot be null. Can be empty if author information could not be parsed.
	/// </summary>
	public string Author { get; set; }

	/// <summary>
	/// Author's Piapro ID (ピアプロID, the URL ending), for example "yakari".
	/// Cannot be null. Can be empty if author information could not be parsed.
	/// </summary>
	public string AuthorId { get; set; }

	/// <summary>
	/// Post publish date.
	/// </summary>
	public DateTime? Date { get; set; }

	/// <summary>
	/// Post ID in the long format, for example "61zc7sceslg04gcx".
	/// Cannot be null or empty.
	/// </summary>
	public string Id { get; set; }

	/// <summary>
	/// Audio length in seconds.
	/// Can be null if the length could not be parsed.
	/// </summary>
	public int? LengthSeconds { get; set; }

	/// <summary>
	/// Type of post.
	/// </summary>
	public PostType PostType { get; set; }

	/// <summary>
	/// Post title, for example "マトリョシカ　オケ".
	/// Cannot be null or empty.
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// The parsed URL.
	/// Cannot be null or empty.
	/// </summary>
	public string Url { get; set; }

	/// <summary>
	/// Upload timestamp. Required for the HTML5 player.
	/// For example "20140906145909"
	/// </summary>
	public string UploadTimestamp { get; set; }
}

public enum PostType
{
	Audio,

	Illustration,

	Other
}