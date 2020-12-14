#nullable disable

using System.Configuration;

namespace VocaDb.Model.Utils.Config
{
	public class AssetsSection : ConfigurationSection
	{
		[ConfigurationProperty("favIconUrl", DefaultValue = null)]
		public string FavIconUrl
		{
			get => (string)this["favIconUrl"];
			set => this["favIconUrl"] = value;
		}
	}
}
