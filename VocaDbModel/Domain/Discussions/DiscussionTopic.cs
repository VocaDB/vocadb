using System;
using System.Collections.Generic;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Discussions {

	public class DiscussionTopic : IEntryWithNames {

		string IEntryBase.DefaultName {
			get { return Title; }
		}

		INameManager IEntryWithNames.Names {
			get {
				return new SingleNameManager(Title);
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

		public DiscussionTopic() { }

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

		public virtual DateTime CreateDate { get; set; }

		public virtual bool Deleted { get; set; }

		public virtual EntryType EntryType {
			get { return EntryType.DiscussionTopic; }
		}

		public virtual DiscussionFolder Folder {
			get { return folder; }
			set {
				ParamIs.NotNull(() => value);
				folder = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual bool Locked { get; set; }

		public virtual bool Pinned { get; set; }

		public virtual string Title {
			get { return title; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				title = value;
			}
		}

	}

}
