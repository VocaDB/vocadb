#nullable disable

using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ReleaseEventForEditContract : ReleaseEventDetailsContract
	{
		public ReleaseEventForEditContract()
		{
			Names = new LocalizedStringWithIdContract[0];
		}

		public ReleaseEventForEditContract(ReleaseEvent releaseEvent, ContentLanguagePreference languagePreference, IUserPermissionContext userContext, IUserIconFactory userIconFactory) :
			base(releaseEvent, languagePreference, userContext, userIconFactory)
		{
			Names = releaseEvent.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
		}

		public LocalizedStringWithIdContract[] Names { get; set; }
	}
}
