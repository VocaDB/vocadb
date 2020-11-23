using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VocaDb.Model;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Admin
{
	[JsonObject(MemberSerialization.OptIn)]
	public class ViewAuditLogModel
	{
		public ViewAuditLogModel()
		{
			GroupId = AuditLogUserGroupFilter.NoFilter;

			UserGroups = new[] {
				new KeyValuePair<AuditLogUserGroupFilter, string>(AuditLogUserGroupFilter.NoFilter, "No group filter")
				}.Concat(Translate.UserGroups.Values.Select(u => new KeyValuePair<AuditLogUserGroupFilter, string>(EnumVal<AuditLogUserGroupFilter>.Parse(u.ToString()), Translate.UserGroups[u])))
				.ToArray();
		}

		[JsonProperty]
		public string ExcludeUsers { get; set; }

		[JsonProperty]
		public string Filter { get; set; }

		[JsonProperty]
		public AuditLogUserGroupFilter GroupId { get; set; }

		[JsonProperty]
		public bool OnlyNewUsers { get; set; }

		public KeyValuePair<AuditLogUserGroupFilter, string>[] UserGroups { get; set; }

		[JsonProperty]
		public string UserName { get; set; }
	}
}