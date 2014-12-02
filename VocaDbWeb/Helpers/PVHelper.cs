using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.Helpers {

	public static class PVHelper {

		public static PVService? PreferredVideoService {
			get {
				return MvcApplication.LoginManager.IsLoggedIn ? (PVService?)MvcApplication.LoginManager.LoggedUser.PreferredVideoService : null;
			}
		}

		public static PVContract[] GetMainPVs(PVContract[] allPvs) {

			return EnumVal<PVService>.Values.Select(service => VideoServiceHelper.GetPV(allPvs, service)).Where(p => p != null).ToArray();

		}

		public static string GetNicoId(IEnumerable<PVContract> pvs, string nicoId) {

			var nicoPV = pvs.FirstOrDefault(p => p.Service == PVService.NicoNicoDouga && p.PVType == PVType.Original);

			if (nicoPV != null)
				return nicoPV.PVId;

			return nicoId;

		}

		public static PVContract PrimaryPV(IEnumerable<PVContract> pvs) {

			return VideoServiceHelper.PrimaryPV(pvs, PreferredVideoService);

		}

	}
}