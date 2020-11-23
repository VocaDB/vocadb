using System.Xml.Serialization;
using Newtonsoft.Json;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Models.Ext
{

	[XmlRoot(ElementName = "oembed")]
	public class SongOEmbedResponse
	{

		public SongOEmbedResponse() { }

		public SongOEmbedResponse(SongForApiContract song, int width, int height, string html)
		{

			ParamIs.NotNull(() => song);
			ParamIs.NotNullOrEmpty(() => html);

			AuthorName = song.ArtistString;
			ThumbnailUrl = song.ThumbUrl;
			Title = song.Name;

			Height = height;
			Html = html;
			Width = width;

		}

		[JsonProperty("author_name"), XmlElement("author_name")]
		public string AuthorName { get; set; }

		[JsonProperty("height"), XmlElement("height")]
		public int Height { get; set; }

		[JsonProperty("html"), XmlElement("html")]
		public string Html { get; set; }

		[JsonProperty("provider_name"), XmlElement("provider_name")]
		public string ProviderName => "VocaDB";

		[JsonProperty("provider_url"), XmlElement("provider_url")]
		public string ProviderUrl => AppConfig.HostAddress;

		[JsonProperty("thumbnail_url"), XmlElement("thumbnail_url")]
		public string ThumbnailUrl { get; set; }

		[JsonProperty("title"), XmlElement("title")]
		public string Title { get; set; }

		[JsonProperty("type"), XmlElement("type")]
		public string Type => "video";

		[JsonProperty("version"), XmlElement("version")]
		public string Version => "1.0";

		[JsonProperty("width"), XmlElement("width")]
		public int Width { get; set; }

	}

}