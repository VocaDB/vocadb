using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace VocaDb.Model.Domain.Security {

	public class PermissionCollection : IEnumerable<PermissionToken> {

		public static PermissionCollection operator +(PermissionCollection left, PermissionCollection right) {
			return left.Merge(right);
		}

		private ISet<PermissionToken> permissions;

		private void AddAll(IEnumerable<PermissionToken> flags) {

			foreach (var flag in flags)
				permissions.Add(flag);

		}

		public PermissionCollection() {
			permissions = new HashSet<PermissionToken>();
		}

		public PermissionCollection(IEnumerable<PermissionToken> permissions) {
			this.permissions = new HashSet<PermissionToken>();
			AddAll(permissions);
		}

		public PermissionCollection(ICollection<PermissionToken> permissions) {
			this.permissions = new HashSet<PermissionToken>(permissions);
		}

		public void Add(PermissionToken permissionToken) {
			permissions.Add(permissionToken);
		}

		public ISet<PermissionToken> Permissions {
			get { return permissions; }
			protected set {
				ParamIs.NotNull(() => value);
				permissions = value;
			}
		}

		public IEnumerable<PermissionToken> PermissionTokens {
			get { return permissions; }
		}

		public IEnumerator<PermissionToken> GetEnumerator() {
			return PermissionTokens.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public bool Has(PermissionToken flag) {

			return (flag == PermissionToken.Nothing || permissions.Contains(flag));

		}

		public PermissionCollection Merge(PermissionCollection collection) {

			ParamIs.NotNull(() => collection);

			if (!collection.Permissions.Any())
				return this;

			return new PermissionCollection(Permissions.Concat(collection.Permissions));

		}

	}
}
