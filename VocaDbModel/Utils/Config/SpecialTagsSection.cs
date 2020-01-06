using System;
using System.Configuration;

namespace VocaDb.Model.Utils.Config {

	public class SpecialTagsSection : ConfigurationSection, ISpecialTags {

		private int TagId(string name) {
			return this.Properties.Contains(name) ? (int)this[name] : 0;
		}

		[ConfigurationProperty("changedLyrics")]
		public int ChangedLyrics {
			get { return TagId("changedLyrics"); }
			set { this["changedLyrics"] = value; }
		}

		[ConfigurationProperty("free")]
		public int Free
		{
			get { return TagId("free"); }
			set { this["free"] = value; }
		}

		[ConfigurationProperty("shortVersion")]
		public int ShortVersion {
			get { return TagId("shortVersion"); }
			set { this["shortVersion"] = value; }
		}

	}

	public interface ISpecialTags {
		int ChangedLyrics { get; }
	}

}
