using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Tags {
	
	/// <summary>
	/// Manages tags for an entry.
	/// </summary>
	/// <typeparam name="T">Tag usage type.</typeparam>
	public class TagManager<T> where T : TagUsage {

		private ISet<T> tags = new HashSet<T>();

		public virtual IEnumerable<Tag> Tags {
			get {
				return Usages.Select(t => t.Tag);
			}
		}

		/// <summary>
		/// Tags sorted descending by the number of votes. Cannot be null.
		/// </summary>
		public virtual IEnumerable<Tag> TagsByVotes {
			get {
				return Usages.OrderByDescending(u => u.Count).Select(u => u.Tag);
			}
		}

		/// <summary>
		/// List of all tag usages. Cannot be null.
		/// </summary>
		public virtual ISet<T> Usages {
			get { return tags; }
			set {
				ParamIs.NotNull(() => value);
				tags = value;
			}
		}

		/// <summary>
		/// Deletes all tag usages, updating caches.
		/// </summary>
		public virtual void DeleteUsages() {

			var list = Usages.ToArray();

			foreach (var usage in list)
				usage.Delete();

		}

		public virtual T GetTagUsage(Tag tag) {
			ParamIs.NotNull(() => tag);
			return Usages.FirstOrDefault(t => t.Tag.Equals(tag));
		}

		public virtual bool HasTag(Tag tag) {

			ParamIs.NotNull(() => tag);

			return Usages.Any(u => u.Tag.Equals(tag));

		}

		public virtual bool HasTag(int tagId) {

			return Usages.Any(u => u.Tag.Id == tagId);

		}

		public virtual void SyncVotes(User user, Tag[] tags, ITagUsageFactory<T> tagUsageFactory,
			bool onlyAdd = false) {

			var actualTags = tags.Select(t => t.ActualTag).Distinct().ToArray();
			var tagUsagesDiff = CollectionHelper.Diff(Usages, actualTags, (t1, t2) => t1.Tag.Equals(t2));

			foreach (var newUsageTag in tagUsagesDiff.Added) {
				var newUsage = tagUsageFactory.CreateTagUsage(newUsageTag);
				Usages.Add(newUsage);
				newUsage.CreateVote(user);
			}

			if (!onlyAdd) {
				foreach (var removedTag in tagUsagesDiff.Removed) {

					removedTag.RemoveVote(user);

					if (removedTag.Count <= 0)
						Usages.Remove(removedTag);

				}				
			}

			foreach (var updated in tagUsagesDiff.Unchanged) {

				updated.CreateVote(user);

			}

		}

	}

}
