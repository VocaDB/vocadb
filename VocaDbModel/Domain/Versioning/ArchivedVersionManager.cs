using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Versioning {

	public interface IArchivedVersionsManager {

		IEnumerable<ArchivedObjectVersion> VersionsBase { get; }

		ArchivedObjectVersion GetLatestVersion();

		bool HasAny();

	}

	public class ArchivedVersionManager<TVersion, TField> : 
		IArchivedVersionsManager
		where TVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<TField> 
		where TField : struct, IConvertible {

		private IList<TVersion> archivedVersions = new List<TVersion>();

		public virtual IList<TVersion> Versions {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public IEnumerable<ArchivedObjectVersion> VersionsBase => Versions;

		public virtual TVersion Add(TVersion newVersion) {

			ParamIs.NotNull(() => newVersion);

			Versions.Add(newVersion);
			return newVersion;

		}

		public virtual void Clear() {
			Versions.Clear();
		}

		ArchivedObjectVersion IArchivedVersionsManager.GetLatestVersion() {
			return GetLatestVersion();
		}

		public virtual TVersion GetLatestVersion() {

			// Sort first by version number because it's more accurate.
			// Also need to sort by creation date because version number is not available for all entry types.
			return Versions
				.OrderByDescending(m => m.Version)
				.ThenByDescending(m => m.Created)
				.FirstOrDefault();

		}

		/// <summary>
		/// Gets the latest version containing a specified field.
		/// </summary>
		/// <param name="field">The specified field.</param>
		/// <param name="lastVersion">Version number to be compared to. Only this version and earlier ones are considered.</param>
		/// <returns>Version containing the specified field, with a version number lower than the specified version number.</returns>
		/// <remarks>
		/// This method can be used to construct the current state of an entry from earlier versions. 
		/// Not every version contains every field, so when constructing the current state of an entry, 
		/// the latest version containing each field must be processed.
		/// </remarks>
		public virtual TVersion GetLatestVersionWithField(TField field, int lastVersion) {

			return Versions
				.Where(a => a.Version <= lastVersion && a.IsIncluded(field))
				.OrderByDescending(m => m.Version)
				.FirstOrDefault();

		}

		/// <summary>
		/// Gets versions before the specified version.
		/// </summary>
		/// <param name="beforeVer">Version to be compared. Can be null in which case all versions are returned.</param>
		/// <returns>Versions whose number is lower than the compared version. Cannot be null.</returns>
		public virtual IEnumerable<TVersion> GetPreviousVersions(TVersion beforeVer, IUserPermissionContext permissionContext) {

			if (beforeVer == null)
				return Versions;

			return Versions
				.Where(a => a.Version < beforeVer.Version)
				.Where(v => permissionContext.HasPermission(PermissionToken.ViewHiddenRevisions) || !v.Hidden);

		}

		public virtual TVersion GetVersion(int ver) {

			return Versions.FirstOrDefault(v => v.Version == ver);

		}

		public virtual bool HasAny() {
			return Versions.Any();
		}

	}

}
