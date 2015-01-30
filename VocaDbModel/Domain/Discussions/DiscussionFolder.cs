using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Discussions {

	public class DiscussionFolder {

		private string description;
		private string title;
		private IList<DiscussionTopic> topics = new List<DiscussionTopic>();

		public DiscussionFolder() {
			Description = string.Empty;
		}

		/// <summary>
		/// List of topics for this folder.
		/// This list includes deleted topics.
		/// </summary>
		public virtual IList<DiscussionTopic> AllTopics {
			get { return topics; }
			set {
				ParamIs.NotNull(() => value);
				topics = value;
			}
		}

		public virtual bool Deleted { get; set; }

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return title; }
			set {
				ParamIs.NotNull(() => value);
				title = value;
			}
		}

		public virtual UserGroupId RequiredGroup { get; set; }

		public virtual int SortIndex { get; set; }

		/// <summary>
		/// List of discussion topics for this folder.
		/// This list does not include deleted topics.
		/// </summary>
		public virtual IEnumerable<DiscussionTopic> Topics {
			get {
				return AllTopics.Where(t => !t.Deleted);
			}
		}

		public override string ToString() {
			return string.Format("Discussion folder '{0}' [{1}]", Name, Id);
		}

	}

}
