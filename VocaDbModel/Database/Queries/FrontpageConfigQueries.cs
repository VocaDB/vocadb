using System.Runtime.Caching;
using Newtonsoft.Json;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Queries;

public sealed class FrontpageConfigQueries
{
	private readonly IUserPermissionContext _permissionContext;
	private readonly IRepository _repo;
	private readonly ObjectCache _cache;

	public FrontpageConfigQueries(
		IUserPermissionContext permissionContext,
		IRepository repo,
		ObjectCache cache)
	{
		_permissionContext = permissionContext;
		_repo = repo;
		_cache = cache;
	}

	public FrontpageConfigContract GetFrontpageConfig()
	{
		return _cache.GetOrInsert(
			"FrontpageConfig.Data",
			CachePolicy.Never(),
			() => _repo.HandleQuery(ctx =>
			{
				var config = ctx.Query<Config>()
					.FirstOrDefault(c => c.Type == ConfigType.Frontpage);

				if (config?.Value == null)
				{
					return new FrontpageConfigContract
					{
						Banners = Array.Empty<FrontpageBannerContract>()
					};
				}

				return JsonConvert.DeserializeObject<FrontpageConfigContract>(config.Value)
					?? new FrontpageConfigContract { Banners = Array.Empty<FrontpageBannerContract>() };
			})
		);
	}

	public void UpdateFrontpageConfig(FrontpageConfigContract contract)
	{
		_permissionContext.VerifyPermission(PermissionToken.Admin);

		_repo.HandleTransaction(ctx =>
		{
			ctx.AuditLogger.SysLog("updating frontpage configuration");

			var config = ctx.Query<Config>()
				.FirstOrDefault(c => c.Type == ConfigType.Frontpage);

			var json = JsonConvert.SerializeObject(contract);

			if (config == null)
			{
				config = new Config(ConfigType.Frontpage, json);
				ctx.Save(config);
			}
			else
			{
				config.Value = json;
				config.Updated = DateTime.Now;
				ctx.Update(config);
			}

			// Clear cache
			_cache.Remove("FrontpageConfig.Data");

			ctx.AuditLogger.AuditLog("updated frontpage configuration");
		});
	}
}
