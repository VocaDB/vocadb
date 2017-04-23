using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventForEditContract : ReleaseEventDetailsContract {

		public ReleaseEventForEditContract() { }
		public ReleaseEventForEditContract(ReleaseEvent releaseEvent, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory) : 
			base(releaseEvent, languagePreference, userIconFactory) {

			Names = releaseEvent.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();

		}

		public LocalizedStringWithIdContract[] Names { get; set; }

	}

}
