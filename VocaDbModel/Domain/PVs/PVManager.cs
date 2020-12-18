#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.PVs
{
	/// <summary>
	/// Manages a collection of PVs.
	/// </summary>
	public class PVManager<T> : IEnumerable<T> where T : class, IEditablePV
	{
		private IList<T> pvs = new List<T>();

		public virtual IList<T> PVs
		{
			get => pvs;
			set
			{
				ParamIs.NotNull(() => value);
				pvs = value;
			}
		}

		public virtual T Add(T pv)
		{
			PVs.Add(pv);
			return pv;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public virtual IEnumerator<T> GetEnumerator()
		{
			return PVs.GetEnumerator();
		}

		public virtual IEnumerable<T> OfType(PVType pvType)
		{
			return PVs.Where(p => p.PVType == pvType);
		}

		public virtual void Remove(T pv)
		{
			PVs.Remove(pv);
		}

		public virtual CollectionDiffWithValue<T, T> Sync(IList<PVContract> newPVs, Func<PVContract, T> fac)
		{
			ParamIs.NotNull(() => newPVs);

			var diff = CollectionHelper.Diff(PVs, newPVs, (n1, n2) => n1.Id == n2.Id);
			var created = new List<T>();
			var edited = new List<T>();

			foreach (var n in diff.Removed)
			{
				n.OnDelete();
			}

			foreach (var newEntry in diff.Added)
			{
				var l = fac(newEntry);
				created.Add(l);
			}

			foreach (var linkEntry in diff.Unchanged)
			{
				var entry = linkEntry;
				var newEntry = newPVs.First(e => e.Id == entry.Id);

				if (!entry.ContentEquals(newEntry))
				{
					linkEntry.CopyMetaFrom(newEntry);
					edited.Add(linkEntry);
				}
			}

			// Already done at add/delete
			//UpdateNicoId();
			//UpdatePVServices();

			return new CollectionDiffWithValue<T, T>(created, diff.Removed, diff.Unchanged, edited);
		}
	}
}
