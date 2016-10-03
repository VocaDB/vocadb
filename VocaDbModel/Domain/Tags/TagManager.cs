using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Tags {
	
	/// <summary>
	/// Manages tags and tag usages for an entry.
	/// There's always one tag manager per entry.
	/// </summary>
	/// <typeparam name="T">Type of tag usage.</typeparam>
	public class TagManager<T> : ITagManager where T : TagUsage {

		private ISet<T> tags = new HashSet<T>();

		public virtual IEnumerable<T> ActiveUsages => Usages.Where(t => !t.Tag.Deleted);

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

		public virtual Tag[] SyncVotes(User user, Tag[] tags, ITagUsageFactory<T> tagUsageFactory,
			bool onlyAdd = false) {

			var actualTags = tags.Distinct().ToArray();
			var tagUsagesDiff = CollectionHelper.Diff(Usages, actualTags, (t1, t2) => t1.Tag.Equals(t2));
			var modifiedTags = new List<Tag>(tagUsagesDiff.Added.Length + tagUsagesDiff.Removed.Length + tagUsagesDiff.Unchanged.Length);

			foreach (var newUsageTag in tagUsagesDiff.Added) {
				var newUsage = tagUsageFactory.CreateTagUsage(newUsageTag);
				Usages.Add(newUsage);
				newUsage.CreateVote(user);
				newUsageTag.UsageCount++;
				modifiedTags.Add(newUsageTag);
			}

			if (!onlyAdd) {
				foreach (var removedTag in tagUsagesDiff.Removed) {

					removedTag.RemoveVote(user);

					if (!removedTag.HasVotes) {
						removedTag.Tag.UsageCount--;
						Usages.Remove(removedTag);
					}

					modifiedTags.Add(removedTag.Tag);

				}				
			}

			foreach (var updated in tagUsagesDiff.Unchanged) {

				updated.CreateVote(user);
				modifiedTags.Add(updated.Tag);

			}

			return modifiedTags.ToArray();

		}

	}

	public interface ITagManager {

		IEnumerable<Tag> Tags { get; }

		bool HasTag(Tag tag);

	}

}
