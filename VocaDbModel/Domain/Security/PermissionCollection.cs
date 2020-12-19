#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace VocaDb.Model.Domain.Security
{
	public interface IPermissionCollection : IEnumerable<PermissionToken>
	{
		bool Has(PermissionToken flag);
	}

	public class PermissionCollection : IPermissionCollection
	{
		public static PermissionCollection operator +(PermissionCollection left, PermissionCollection right)
		{
			return left.Merge(right);
		}

		public static readonly PermissionCollection Empty = new();

		private ISet<PermissionToken> _permissions;

		private void AddAll(IEnumerable<PermissionToken> flags)
		{
			foreach (var flag in flags)
				_permissions.Add(flag);
		}

		public PermissionCollection()
		{
			_permissions = new HashSet<PermissionToken>();
		}

		public PermissionCollection(IEnumerable<PermissionToken> permissions)
		{
			this._permissions = new HashSet<PermissionToken>();
			AddAll(permissions);
		}

		public PermissionCollection(ICollection<PermissionToken> permissions)
		{
			this._permissions = new HashSet<PermissionToken>(permissions);
		}

		public void Add(PermissionToken permissionToken)
		{
			_permissions.Add(permissionToken);
		}

		public ISet<PermissionToken> Permissions
		{
			get => _permissions;
			protected set
			{
				ParamIs.NotNull(() => value);
				_permissions = value;
			}
		}

		public IEnumerable<PermissionToken> PermissionTokens => _permissions;

		public IEnumerator<PermissionToken> GetEnumerator()
		{
			return PermissionTokens.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Has(PermissionToken flag)
		{
			return (flag == PermissionToken.Nothing || _permissions.Contains(flag));
		}

		public PermissionCollection Merge(PermissionCollection collection)
		{
			ParamIs.NotNull(() => collection);

			if (!collection.Permissions.Any())
				return this;

			return new PermissionCollection(Permissions.Concat(collection.Permissions));
		}

		public PermissionCollection Merge(IPermissionCollection collection)
		{
			ParamIs.NotNull(() => collection);
			return new PermissionCollection(this.Concat(collection));
		}
	}
}
