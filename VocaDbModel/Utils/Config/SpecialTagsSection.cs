using System.Configuration;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Utils.Config {

	public class SpecialTagsSection : ConfigurationSection {

		private int TagId(string name) {
			return this.Properties.Contains(name) ? (int)this[name] : 0;
		}

		[ConfigurationProperty("changedLyrics")]
		public int ChangedLyrics
		{
			get { return TagId("changedLyrics"); }
			set { this["changedLyrics"] = value; }
		}

		[ConfigurationProperty("free")]
		public int Free
		{
			get { return TagId("free"); }
			set { this["free"] = value; }
		}

		[ConfigurationProperty("instrumental")]
		public int Instrumental
		{
			get { return TagId("instrumental"); }
			set { this["instrumental"] = value; }
		}

		public int GetSpecialTagId(SpecialTagType specialTag) {

			switch (specialTag) {
				case SpecialTagType.ChangedLyrics:
					return ChangedLyrics;
				case SpecialTagType.Free:
					return Free;
				case SpecialTagType.Instrumental:
					return Instrumental;
				default:
					return 0;
			}

		}

	}

}
