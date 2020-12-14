#nullable disable

using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.Helpers
{
	public static class PVHelper
	{
		private static PVService? PreferredVideoService
		{
			get
			{
				return MvcApplication.LoginManager.IsLoggedIn ? (PVService?)MvcApplication.LoginManager.LoggedUser.PreferredVideoService : null;
			}
		}

		public static PVContract[] GetMainPVs(PVContract[] allPvs)
		{
			return EnumVal<PVService>.Values.Select(service => VideoServiceHelper.GetPV(allPvs, service)).Where(p => p != null).ToArray();
		}

		public static PVContract PrimaryPV(IEnumerable<PVContract> pvs)
		{
			return VideoServiceHelper.PrimaryPV(pvs, PreferredVideoService);
		}
	}
}