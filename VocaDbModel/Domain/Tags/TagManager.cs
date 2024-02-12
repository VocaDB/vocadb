using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Tags;

/// <summary>
/// Manages tags and tag usages for an entry.
/// There's always one tag manager per entry.
/// </summary>
/// <typeparam name="T">Type of tag usage.</typeparam>
public class TagManager<T> : ITagManager where T : TagUsage
{
	private ISet<T> _tags = new HashSet<T>();

	/// <summary>
	/// Usages of tags that are not deleted.
	/// </summary>
	public virtual IEnumerable<T> ActiveUsages => Usages.Where(t => !t.Tag.Deleted);

	public virtual IEnumerable<Tag> Tags => Usages.Select(t => t.Tag);

	/// <summary>
	/// Tags sorted descending by the number of votes. Cannot be null.
	/// </summary>
	public virtual IEnumerable<Tag> TagsByVotes => Usages.OrderByDescending(u => u.Count).Select(u => u.Tag);

	/// <summary>
	/// List of all tag usages. Cannot be null.
	/// This property is mapped to database.
	/// </summary>
	public virtual ISet<T> Usages
	{
		get => _tags;
		set
		{
			ParamIs.NotNull(() => value);
			_tags = value;
		}
	}

	/// <summary>
	/// Deletes all tag usages, updating caches.
	/// </summary>
	public virtual void DeleteUsages()
	{
		var list = Usages.ToArray();

		foreach (var usage in list)
			usage.Delete();
	}

	public virtual T? GetTagUsage(Tag tag)
	{
		ParamIs.NotNull(() => tag);
		return Usages.FirstOrDefault(t => t.Tag.Equals(tag));
	}

	public virtual bool HasTag(Tag tag)
	{
		ParamIs.NotNull(() => tag);

		return Usages.Any(u => u.Tag.Equals(tag));
	}

	public virtual bool HasTag(int tagId) => Usages.Any(u => u.Tag.Id == tagId);

	public virtual Tag[] SyncVotes(
		User user,
		Tag[] tags,
		ITagUsageFactory<T> tagUsageFactory,
		bool onlyAdd = false
	)
	{
		var actualTags = tags.Distinct().ToArray();
		var tagUsagesDiff = CollectionHelper.Diff(Usages, actualTags, (t1, t2) => t1.Tag.Equals(t2));
		var modifiedTags = new List<Tag>(tagUsagesDiff.Added.Length + tagUsagesDiff.Removed.Length + tagUsagesDiff.Unchanged.Length);

		foreach (var newUsageTag in tagUsagesDiff.Added)
		{
			var newUsage = tagUsageFactory.CreateTagUsage(newUsageTag);
			Usages.Add(newUsage);
			newUsage.CreateVote(user);
			newUsageTag.UsageCount++;
			modifiedTags.Add(newUsageTag);
		}

		if (!onlyAdd)
		{
			foreach (var removedTag in tagUsagesDiff.Removed)
			{
				removedTag.RemoveVote(user);

				if (!removedTag.HasVotes)
				{
					removedTag.Tag.UsageCount--;
					Usages.Remove(removedTag);
				}

				modifiedTags.Add(removedTag.Tag);
			}
		}

		foreach (var updated in tagUsagesDiff.Unchanged)
		{
			updated.CreateVote(user);
			modifiedTags.Add(updated.Tag);
		}

		return modifiedTags.ToArray();
	}

	public virtual void MoveVotes(TagManager<T> targetManager, Func<Tag, T> tagUsageCreation)
	{
		foreach (var sourceUsage in Usages.ToArray())
		{
			T usage =  targetManager.HasTag(sourceUsage.Tag) ? 
				targetManager.GetTagUsage(sourceUsage.Tag)! : 
				tagUsageCreation(sourceUsage.Tag);
			targetManager.Usages.Add(usage);
			sourceUsage.Tag.UsageCount++;

			foreach (var sourceVote in sourceUsage.VotesBase)
			{
				var vote = usage.CreateVote(sourceVote.User);

				if (vote == null) continue;
			}
		}
	}
}

public interface ITagManager
{
	IEnumerable<Tag> Tags { get; }

	bool HasTag(Tag tag);
}
