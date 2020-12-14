#nullable disable

using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Tags
{
	public class TagName : EntryName<Tag>
	{
		public TagName() { }

		public TagName(Tag song, LocalizedString localizedString)
			: base(song, localizedString) { }
	}
}
