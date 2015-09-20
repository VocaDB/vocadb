using System;
using System.Collections.Generic;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Discussions {

	public class DiscussionTopic : IEntryWithNames, IEntryWithComments {

		IEnumerable<Comment> IEntryWithComments.Comments => Comments;

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		INameManager IEntryWithNames.Names {
			get {
				return new SingleNameManager(Name);
			}
		}

		int IEntryBase.Version {
			get { return 0; }
		}

		private string authorName;
		private IList<DiscussionComment> comments = new List<DiscussionComment>();
		private string content;
		private DiscussionFolder folder;
		private string title;

		public DiscussionTopic() {
		}

		public DiscussionTopic(DiscussionFolder folder, string name, string content, AgentLoginData agent) {

			Folder = folder;
			Name = name;
			Content = content;
			Author = agent.User;
			AuthorName = agent.Name;

			Created = DateTime.Now;

		}

		public virtual User Author { get; set; }

		public virtual string AuthorName {
			get { return authorName; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				authorName = value;
			}
		}

		public virtual IList<DiscussionComment> Comments {
			get { return comments; }
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		public virtual string Content {
			get { return content; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				content = value;
			}
		}

		public virtual DateTime Created { get; set; }

		public virtual bool Deleted { get; set; }

		public virtual EntryType EntryType {
			get { return EntryType.DiscussionTopic; }
		}

		/// <summary>
		/// Folder containing this topic. Cannot be null.
		/// </summary>
		public virtual DiscussionFolder Folder {
			get { return folder; }
			set {
				ParamIs.NotNull(() => value);
				folder = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual bool Locked { get; set; }

		public virtual string Name {
			get { return title; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				title = value;
			}
		}

		public virtual bool Pinned { get; set; }

		public virtual Comment CreateComment(string message, AgentLoginData loginData) {

			var comment = new DiscussionComment(this, message, loginData);
			Comments.Add(comment);
			return comment;

		}

		public override string ToString() {
			return string.Format("Discussion topic '{0}' [{1}]", Name, Id);
		}

	}

}
