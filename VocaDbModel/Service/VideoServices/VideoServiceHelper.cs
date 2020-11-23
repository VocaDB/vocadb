using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service.VideoServices
{
	public static class VideoServiceHelper
	{
		private static readonly VideoService[] services = {
			VideoService.Bandcamp,
 			VideoService.Bilibili,
			VideoService.NicoNicoDouga,
			VideoService.Piapro,
			VideoService.SoundCloud,
			VideoService.Youtube,
			VideoService.Vimeo,
			VideoService.File,
			VideoService.LocalFile,
			VideoService.Creofuga,
		};

		public static readonly Dictionary<PVService, VideoService> Services = services.ToDictionary(s => s.Service);

		private static readonly ImmutableHashSet<PVService> servicesWithoutExternalSiteLink =
			ImmutableHashSet.Create(PVService.File, PVService.LocalFile);

		public static bool ShowExternalSiteLink(PVService service)
		{
			return !servicesWithoutExternalSiteLink.Contains(service);
		}

		public static T GetPV<T>(T[] allPvs, PVService service)
			where T : class, IPV
		{
			var servicePvs = allPvs.Where(p => p.Service == service).ToArray();

			return GetPV(servicePvs, true,
				p => p.PVType == PVType.Original,
				p => p.PVType == PVType.Reprint);
		}

		public static T GetPV<T>(T[] allPvs) where T : class, IPV
		{
			return GetPV(allPvs, true,
				p => p.PVType == PVType.Original,
				p => p.PVType == PVType.Reprint);
		}

		public static T GetPV<T>(ICollection<T> allPvs, bool acceptFirst, params Func<T, bool>[] predicates) where T : class, IPV
		{
			if (!allPvs.Any())
				return null;

			foreach (var predicate in predicates)
			{
				var pv = allPvs.FirstOrDefault(p => predicate(p));

				if (pv != null)
					return pv;
			}

			return acceptFirst ? allPvs.FirstOrDefault() : null;
		}

		public static string GetThumbUrl(IPVWithThumbnail pv) => Services[pv.Service].GetThumbUrlById(pv.PVId);

		public static string GetThumbUrl<T>(IList<T> pvs) where T : class, IPVWithThumbnail
		{
			ParamIs.NotNull(() => pvs);

			if (!pvs.Any())
				return string.Empty;

			var pvsWithThumb = pvs.Where(p => !string.IsNullOrEmpty(p.ThumbUrl)).OrderBy(p => (int)p.PVType);

			var pv =
				pvsWithThumb.Where(p => !p.Disabled).FirstOrDefault() ??
				pvsWithThumb.FirstOrDefault() ??
				pvs.FirstOrDefault(p => p.PVType == PVType.Original) ??
				pvs.FirstOrDefault();

			return (pv != null ? (!string.IsNullOrEmpty(pv.ThumbUrl) ? pv.ThumbUrl : GetThumbUrl(pv)) : string.Empty);
		}

		/// <summary>
		/// Gets a thumb URL, preferring a service that's not NND (because nico thumbs don't work in RSS feeds etc.)
		/// </summary>
		/// <param name="pvs">List of PVs. Cannot be null.</param>
		/// <returns>Thumb URL. Cannot be null. Can be empty if there's no PV.</returns>
		public static string GetThumbUrlPreferNotNico<T>(IList<T> pvs) where T : class, IPVWithThumbnail
		{
			ParamIs.NotNull(() => pvs);

			if (!pvs.Any())
				return string.Empty;

			var notNico = pvs.Where(p => p.Service != PVService.NicoNicoDouga).ToArray();

			var pv = notNico.FirstOrDefault(p => !string.IsNullOrEmpty(p.ThumbUrl) && (p.PVType == PVType.Original || p.PVType == PVType.Reprint) && !p.Disabled);

			if (pv == null)
				pv = notNico.FirstOrDefault(p => !string.IsNullOrEmpty(p.ThumbUrl) && !p.Disabled);

			if (pv == null)
				pv = pvs.FirstOrDefault(p => !string.IsNullOrEmpty(p.ThumbUrl) && !p.Disabled);

			if (pv == null)
				pv = pvs.FirstOrDefault();

			return (pv != null ? (!string.IsNullOrEmpty(pv.ThumbUrl) ? pv.ThumbUrl : GetThumbUrl(pv)) : string.Empty);
		}

		public static string GetMaxSizeThumbUrl<T>(IList<T> pvs) where T : class, IPVWithThumbnail
		{
			ParamIs.NotNull(() => pvs);

			var pv = GetPV(pvs, false,
				p => p.Service == PVService.Youtube && p.PVType == PVType.Original,
				p => p.Service == PVService.Youtube && p.PVType == PVType.Reprint,
				p => p.Service == PVService.Youtube);

			if (pv != null)
			{
				return VideoService.Youtube.GetMaxSizeThumbUrlById(pv.PVId);
			}

			return null;
		}

		public static T PrimaryPV<T>(IEnumerable<T> pvs, PVService? preferredService = null)
			where T : class, IPV
		{
			ParamIs.NotNull(() => pvs);

			var p = pvs.Where(pv => !pv.Disabled).ToArray();

			if (preferredService.HasValue)
				return GetPV(p, preferredService.Value) ?? GetPV(p);
			else
				return GetPV(p);
		}

		public static Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle, IUserPermissionContext permissionContext)
		{
			return ParseByUrlAsync(url, getTitle, permissionContext, services);
		}

		public static Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle, IUserPermissionContext permissionContext, params VideoService[] testServices)
		{
			var service = testServices.FirstOrDefault(s => s.IsAuthorized(permissionContext) && s.IsValidFor(url));

			if (service == null)
			{
				return Task.FromResult(VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.NoMatcher));
			}

			return service.ParseByUrlAsync(url, getTitle);
		}
	}
}
