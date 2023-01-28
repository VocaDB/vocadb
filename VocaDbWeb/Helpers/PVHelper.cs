using VocaDb.Model;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.Helpers;

public class PVHelper
{
	private readonly LoginManager _manager;

	public PVHelper(LoginManager manager)
	{
		_manager = manager;
	}

	public LoginManager LoginManager => _manager;

	private PVService? PreferredVideoService => LoginManager.IsLoggedIn
		? LoginManager.LoggedUser.PreferredVideoService
		: null;

	public PVContract[] GetMainPVs(PVContract[] allPvs)
	{
		return EnumVal<PVService>.Values
			.Select(service => VideoServiceHelper.GetPV(allPvs, service))
			.WhereNotNull()
			.ToArray();
	}

	public PVContract? PrimaryPV(IEnumerable<PVContract> pvs)
	{
		return VideoServiceHelper.PrimaryPV(pvs, PreferredVideoService);
	}
}