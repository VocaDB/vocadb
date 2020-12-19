#nullable disable

using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Discussions
{
	public class DiscussionFolder : IEntryWithIntId
	{
		private string _description;
		private string _title;
		private IList<DiscussionTopic> _topics = new List<DiscussionTopic>();

		public DiscussionFolder()
		{
			Description = string.Empty;
		}

		public DiscussionFolder(string title)
			: this()
		{
			Name = title;
		}

		/// <summary>
		/// List of topics for this folder.
		/// This list includes deleted topics.
		/// </summary>
		public virtual IList<DiscussionTopic> AllTopics
		{
			get => _topics;
			set
			{
				ParamIs.NotNull(() => value);
				_topics = value;
			}
		}

		public virtual bool Deleted { get; set; }

		public virtual string Description
		{
			get => _description;
			set
			{
				ParamIs.NotNull(() => value);
				_description = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual string Name
		{
			get => _title;
			set
			{
				ParamIs.NotNull(() => value);
				_title = value;
			}
		}

		public virtual UserGroupId RequiredGroup { get; set; }

		public virtual int SortIndex { get; set; }

		/// <summary>
		/// List of discussion topics for this folder.
		/// This list does not include deleted topics.
		/// </summary>
		public virtual IEnumerable<DiscussionTopic> Topics => AllTopics.Where(t => !t.Deleted);

		public override string ToString()
		{
			return $"Discussion folder '{Name}' [{Id}]";
		}
	}
}
