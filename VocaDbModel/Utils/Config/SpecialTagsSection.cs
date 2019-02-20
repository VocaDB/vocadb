using System.Collections.Generic;
using System.Configuration;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Utils.Config {

	public class SpecialTagsSection : ConfigurationSection, ISpecialTags {

		private int TagId(string name) => this.Properties.Contains(name) ? (int)this[name] : 0;

		[ConfigurationProperty("changedLyrics")]
		public int ChangedLyrics {
			get { return TagId("changedLyrics"); }
			set { this["changedLyrics"] = value; }
		}

		[ConfigurationProperty("cover")]
		public int Cover {
			get { return TagId("cover"); }
			set { this["cover"] = value; }
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

		[ConfigurationProperty("outOfScope")]
		public int OutOfScope {
			get { return TagId("outOfScope"); }
			set { this["outOfScope"] = value; }
		}

		[ConfigurationProperty("remix")]
		public int Remix {
			get => TagId("remix");
			set => this["remix"] = value;
		}

		[ConfigurationProperty("shortVersion")]
		public int ShortVersion {
			get { return TagId("shortVersion"); }
			set { this["shortVersion"] = value; }
		}

		public int GetSpecialTagId(SpecialTagType specialTag) {

			switch (specialTag) {
				case SpecialTagType.Cover:
					return Cover;
				case SpecialTagType.ChangedLyrics:
					return ChangedLyrics;
				case SpecialTagType.Free:
					return Free;
				case SpecialTagType.Instrumental:
					return Instrumental;
				case SpecialTagType.Remix:
					return Remix;
				case SpecialTagType.ShortVersion:
					return ShortVersion;
				case SpecialTagType.OutOfScope:
					return OutOfScope;
				default:
					return 0;
			}

		}

	}

	public interface ISpecialTags {
		int Cover { get; }
		int ChangedLyrics { get; }
		int Instrumental { get; }
		int Remix { get; }
	}

}
