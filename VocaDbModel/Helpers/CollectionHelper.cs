using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Helpers {

	public static class CollectionHelper {

		public static T AddAndReturn<T>(IList<T> collection, T item) {
			collection.Add(item);
			return item;
		}

		/// <summary>
		/// Calculates a diff between two collections, including new, unchanged and deleted items.
		/// </summary>
		/// <typeparam name="T">Type of the original (current) collection.</typeparam>
		/// <typeparam name="T2">Type of the new collection.</typeparam>
		/// <param name="old">Original (current) collection. Cannot be null.</param>
		/// <param name="newItems">New collection. Cannot be null.</param>
		/// <param name="equality">Equality test. Cannot be null.</param>
		/// <returns>Diff for the two collections. Cannot be null.</returns>
		public static CollectionDiff<T, T2> Diff<T, T2>(IEnumerable<T> old, IEnumerable<T2> newItems, Func<T, T2, bool> equality) {

			ParamIs.NotNull(() => old);
			ParamIs.NotNull(() => newItems);
			ParamIs.NotNull(() => equality);

			var removed = old.Where(i => !newItems.Any(i2 => equality(i, i2)));
			var added = newItems.Where(i => !old.Any(i2 => equality(i2, i)));
			var unchanged = old.Except(removed);	// FIXME: possible error, can't use default equality?

			return new CollectionDiff<T, T2>(added, removed, unchanged);

		}

		/// <summary>
		/// Gets a number of random items from a list.
		/// The items are guaranteed to be unique, meaning that one item cannot appear twice.
		/// </summary>
		/// <typeparam name="T">Type of list.</typeparam>
		/// <param name="source">Source list to be searched. Cannot be null.</param>
		/// <param name="count">Number of items to be returned. If this is equal or more than the number of items in the source list, the source list will be returned as is.</param>
		/// <returns>A number of random items from the list. Cannot be null.</returns>
		public static IEnumerable<T> GetRandomItems<T>(IList<T> source, int count) {

			ParamIs.NotNull(() => source);

			if (source.Count <= count)
				return source;

			var indices = Enumerable.Range(0, source.Count);	// Unique indices for all items in the list.
			var random = new Random();
			var randomIndices = indices
				.OrderBy(x => random.Next())		// Shuffle the list of indices.
				.Take(count);						// Take the first [count] numbers that should now be random and unique.

			return randomIndices.Select(i => source[i]);	// Take items matching the random indices from the list

		} 

		public static IEnumerable<T> MoveToTop<T>(IEnumerable<T> source, T top) {

			return Enumerable.Repeat(top, 1).Concat(source.Except(Enumerable.Repeat(top, 1)));

		}

		public static IEnumerable<T> MoveToTop<T>(IEnumerable<T> source, T[] top) {

			return top.Concat(source.Except(top));

		}

		/// <summary>
		/// Randomly sort a list.
		/// This generates a copy of the list!
		/// </summary>
		/// <typeparam name="T">Item type.</typeparam>
		/// <param name="source">List to be sorted. Cannot be null.</param>
		/// <returns>Randomly sorted list.</returns>
		/// <remarks>
		/// Adapted from http://stackoverflow.com/a/1262619
		/// </remarks>
		public static IEnumerable<T> RandomSort<T>(this IEnumerable<T> source) {

			var list = source.ToList();

			var rng = new Random();
			int n = list.Count;
			while (n > 1) {
				n--;
				int k = rng.Next(n + 1);
				var value = list[k];
				list[k] = list[n];
				list[n] = value;
			}

			return list;

		}

		public static void RemoveAll<T>(IList<T> list, Func<T, bool> pred) {

			bool changed = true;

			while (changed) {

				changed = false;

				foreach (var item in list) {
					if (pred(item)) {
						list.Remove(item);
						changed = true;
						break;
					}
				}

			}

		}

		public static IEnumerable<T> RemovedItems<T>(IEnumerable<T> old, IEnumerable<T> newItems) {

			return old.Where(i => !newItems.Contains(i));

		}

		public static IEnumerable<T> RemovedItems<T, T2>(IEnumerable<T> old, IEnumerable<T2> newItems, Func<T, T2, bool> equality) {

			return old.Where(i => !newItems.Any(i2 => equality(i, i2)));

		}

		public static IEnumerable<T> SkipNull<T>(params T[] items) where T : class {

			return items.Where(i => i != null);

		}

		public static T[] SortByIds<T>(IEnumerable<T> entries, int[] idList) where T : IEntryWithIntId {
			
			var bucket = new T[idList.Length];
			var idToIndex = Enumerable
				.Range(0, idList.Length)
				.ToDictionary(i => idList[i], i => i);

			foreach (var entry in entries) {

				if (!idToIndex.ContainsKey(entry.Id)) {
					throw new InvalidOperationException(string.Format("No ID mapping found for {0}", entry));
				}

				var idx = idToIndex[entry.Id];
				bucket[idx] = entry;

			}

			return bucket;

		} 

		/// <summary>
		/// Syncs items in one collection with a new set.
		/// Removes missing items from the old collection and adds missing new items.
		/// </summary>
		/// <typeparam name="T">Type of the original (current) collection.</typeparam>
		/// <param name="oldItems">Original (current) collection. Cannot be null.</param>
		/// <param name="newItems">New collection. Cannot be null.</param>
		/// <param name="equality">Equality comparer. Cannot be null.</param>
		/// <param name="remove">
		/// Callback for removing an old item if that didn't exist in the new list. 
		/// The old list is already updated by the algorithm. This is mostly used for cleanup of link objects.
		/// Can be null.
		/// </param>
		/// <returns>Diff for the two collections. Cannot be null.</returns>
		public static CollectionDiff<T> Sync<T>(IList<T> oldItems, IList<T> newItems, IEqualityComparer<T> equality, Action<T> remove = null) {

			return Sync(oldItems, newItems, equality.Equals, t => t, remove);

		}

		/// <summary>
		/// Syncs items in one collection with a new set (create and delete, CD).
		/// Removes missing items from the old collection and adds missing new items.
		/// 
		/// This method only supports immutable items: items will never be updated.
		/// For a version that supports mutable items with updates, use <see cref="SyncWithContent"/>.
		/// </summary>
		/// <typeparam name="T">Type of the original (current) collection.</typeparam>
		/// <typeparam name="T2">Type of the new collection.</typeparam>
		/// <param name="old">Original (current) collection. Cannot be null.</param>
		/// <param name="newItems">New collection. Cannot be null.</param>
		/// <param name="equality">Equality test. Cannot be null.</param>
		/// <param name="create">Factory method for the new item. Cannot be null.</param>
		/// <param name="remove">
		/// Callback for removing an old item if that didn't exist in the new list. 
		/// The old list is already updated by the algorithm. This is mostly used for cleanup of link objects.
		/// Can be null.
		/// </param>
		/// <returns>Diff for the two collections. Cannot be null.</returns>
		public static CollectionDiff<T> Sync<T, T2>(ICollection<T> old, IEnumerable<T2> newItems, Func<T, T2, bool> equality, Func<T2, T> create, Action<T> remove = null) {

			var diff = Diff(old, newItems, equality);
			var created = new List<T>();

			foreach (var removed in diff.Removed) {

				if (remove != null)
					remove(removed);

				// Note: this removes the item from the source collection directly, but not from any other collections.
				old.Remove(removed);

			}

			foreach (var linkEntry in diff.Added) {
				var link = create(linkEntry);
				created.Add(link);
			}

			return new CollectionDiff<T>(created, diff.Removed, diff.Unchanged);

		}

		/// <summary>
		/// Syncs items in one collection with a new set, comparing both identity and value (create, update, delete, CUD).
		/// Removes missing items from the old collection and adds missing new items.
		/// Existing items that have been changed will be updated.
		/// </summary>
		/// <typeparam name="T">Type of the original (current) collection.</typeparam>
		/// <typeparam name="T2">Type of the new collection.</typeparam>
		/// <param name="oldItems">Original (current) collection. Cannot be null.</param>
		/// <param name="newItems">New collection. Cannot be null.</param>
		/// <param name="identityEquality">
		/// Identity equality test. Cannot be null.
		/// 
		/// This should only test the identity, and will be used to determine whether the item is 
		/// completely new, possibly updated or removed.
		/// Use <paramref name="update"/> to determine whether the item state has changed, assuming and existing item
		/// with matching identity is found.
		/// </param>
		/// <param name="create">
		/// Factory method for creating a new item. Cannot be null.
		/// 
		/// Normally the factory method should return the created item,
		/// but if the return value is null, no item is added to collection.
		/// </param>
		/// <param name="update">
		/// Method for updating an existing item. 
		/// First parameter is the old item to be updated and second parameter is the new state. 
		/// Returns true if the old item was updated, or false if the items had equal content already. Cannot be null.</param>
		/// <param name="remove">
		/// Callback for removing an old item if that didn't exist in the new list. 
		/// The old list is already updated by the algorithm. This is mostly used for cleanup of link objects.
		/// Can be null.
		/// </param>
		/// <returns>Diff for the two collections. Cannot be null.</returns>
		public static CollectionDiffWithValue<T, T> SyncWithContent<T, T2>(IList<T> oldItems, IList<T2> newItems, 
			Func<T, T2, bool> identityEquality, Func<T2, T> create, Func<T, T2, bool> update, Action<T> remove) where T : class {

			ParamIs.NotNull(() => oldItems);
			ParamIs.NotNull(() => newItems);
			ParamIs.NotNull(() => identityEquality);

			var diff = Diff(oldItems, newItems, identityEquality);
			var created = new List<T>();
			var edited = new List<T>();

			foreach (var removed in diff.Removed) {

				if (remove != null)
					remove(removed);

				oldItems.Remove(removed);

			}

			foreach (var added in diff.Added) {

				var newObject = create(added);

				if (newObject != null)
					created.Add(newObject);

			}

			foreach (var oldItem in diff.Unchanged) {

				var newItem = newItems.First(i => identityEquality(oldItem, i));

				if (update(oldItem, newItem)) {
					edited.Add(oldItem);
				}

			}

			return new CollectionDiffWithValue<T, T>(created, diff.Removed, diff.Unchanged, edited);

		}

	}

	/// <summary>
	/// Difference between two collections.
	/// </summary>
	/// <typeparam name="T">Type of the old collection.</typeparam>
	/// <typeparam name="T2">Type of the new collection (may be the same as old).</typeparam>
	public class CollectionDiff<T, T2> {

		public CollectionDiff(IEnumerable<T2> added, IEnumerable<T> removed, IEnumerable<T> unchanged) {

			ParamIs.NotNull(() => added);
			ParamIs.NotNull(() => removed);
			ParamIs.NotNull(() => unchanged);

			Added = added.ToArray();
			Removed = removed.ToArray();
			Unchanged = unchanged.ToArray();

		}

		/// <summary>
		/// Entries that didn't exist in the old set but do exist in the new one.
		/// </summary>
		public T2[] Added { get; private set; }

		/// <summary>
		/// Whether the contents of the sets were changed.
		/// </summary>
		public virtual bool Changed => Added.Any() || Removed.Any();

		/// <summary>
		/// Entries that existed in the old set but not in the new.
		/// </summary>
		public T[] Removed { get; private set; }

		/// <summary>
		/// Entries that existed in both old and new sets.
		/// Note: the contents of those entriers might still be changed, depending on equality.
		/// </summary>
		public T[] Unchanged { get; private set; }

	}

	/// <summary>
	/// Difference between two collections of the same type.
	/// </summary>
	/// <typeparam name="T">Type of the collection.</typeparam>
	public class CollectionDiff<T> : CollectionDiff<T, T> {

		public CollectionDiff(IEnumerable<T> added, IEnumerable<T> removed, IEnumerable<T> unchanged) 
			: base(added, removed, unchanged) {}

	}

	/// <summary>
	/// Difference between two collections, including value.
	/// </summary>
	/// <typeparam name="T">Type of the old collection.</typeparam>
	/// <typeparam name="T2">Type of the new collection (may be the same as old).</typeparam>
	public class CollectionDiffWithValue<T, T2> : CollectionDiff<T, T2> {

		public CollectionDiffWithValue(IEnumerable<T2> added, IEnumerable<T> removed, 
			IEnumerable<T> unchanged, IEnumerable<T> edited)
			: base(added, removed, unchanged) {

			ParamIs.NotNull(() => edited);

			Edited = edited.ToArray();

		}

		public override bool Changed => base.Changed || Edited.Any();

		/// <summary>
		/// Entries that existed in both old and new sets AND whose contents were changed.
		/// </summary>
		public T[] Edited { get; private set; }

	}

}
