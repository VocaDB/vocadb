using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts {

	/// <summary>
	/// Entry reference with localized entry title.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryRefWithNameContract : EntryRefContract {

		public EntryRefWithNameContract(IEntryWithNames entry, ContentLanguagePreference languagePreference)
			: base(entry) {

			Name = entry.Names.GetEntryName(languagePreference);

		}

		[DataMember]
		public EntryNameContract Name { get; set; }

	}

}
