#nullable disable

using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class TagSelectionContract
	{
		public TagSelectionContract() { }

		public TagSelectionContract(Tag tag, ContentLanguagePreference languagePreference, bool selected)
		{
			Tag = new TagBaseContract(tag, languagePreference, true);
			Selected = selected;
		}

		public bool Selected { get; init; }

		public TagBaseContract Tag { get; init; }
	}
}
