#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class ArchivedTagVersionDetailsContract
	{
		public ArchivedTagVersionDetailsContract() { }

#nullable enable
		public ArchivedTagVersionDetailsContract(ArchivedTagVersion archived, ArchivedTagVersion? comparedVersion, IUserPermissionContext permissionContext, IUserIconFactory userIconFactory)
		{
			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ArchivedTagVersionContract(archived, userIconFactory);
			ComparedVersion = comparedVersion != null ? new ArchivedTagVersionContract(comparedVersion, userIconFactory) : null;
			ComparedVersionId = comparedVersion != null ? comparedVersion.Id : 0;
			Tag = new TagContract(archived.Tag, permissionContext.LanguagePreference);
			Name = Tag.Name;

			ComparableVersions = archived.Tag.ArchivedVersionsManager
				.GetPreviousVersions(archived, permissionContext)
				.Select(a => ArchivedObjectVersionWithFieldsContract.Create(a, userIconFactory, a.Diff.ChangedFields.Value, a.CommonEditEvent))
				.ToArray();

			Versions = ComparedTagsContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;
		}
#nullable disable

		public ArchivedObjectVersionContract ArchivedVersion { get; init; }

		public ArchivedObjectVersionContract[] ComparableVersions { get; init; }

		public ArchivedObjectVersionContract ComparedVersion { get; init; }

		public int ComparedVersionId { get; init; }

		public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

		public string Name { get; init; }

		public TagContract Tag { get; init; }

		public ComparedTagsContract Versions { get; init; }
	}
}
