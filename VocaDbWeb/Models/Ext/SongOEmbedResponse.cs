using System.Xml.Serialization;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Models.Ext {

	[XmlRoot(ElementName = "oembed")]
	public class SongOEmbedResponse {

		public SongOEmbedResponse() {}

		public SongOEmbedResponse(SongContract song, int width, int height, string html) {

			ParamIs.NotNull(() => song);
			ParamIs.NotNullOrEmpty(() => html);

			author_name = song.ArtistString;	
			thumbnail_url = song.ThumbUrl;
			title = song.Name;

			this.height = height;
			this.html = html;
			this.width = width;

		}

		public string author_name { get; set; }

		public int height { get; set; }

		public string html { get; set; }

		public string provider_name {
			get { return "VocaDB"; }
			set { }
		}

		public string provider_url {
			get { return AppConfig.HostAddress; }
			set { }
		}

		public string thumbnail_url { get; set; }

		public string title { get; set; }

		public string type {
			get { return "video"; }
			set { }
		}

		public string version {
			get { return "1.0"; }
			set { }
		}

		public int width { get; set; }

	}

}