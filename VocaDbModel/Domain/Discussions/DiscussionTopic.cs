using System;
using System.Collections.Generic;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Discussions
{
	public class DiscussionTopic : IEntryWithNames, IEntryWithComments
	{
		IEnumerable<Comment> IEntryWithComments.Comments => Comments;

		string IEntryBase.DefaultName => Name;

		INameManager IEntryWithNames.Names => new SingleNameManager(Name);

		int IEntryBase.Version => 0;

		private string authorName;
		private IList<DiscussionComment> comments = new List<DiscussionComment>();
		private string content;
		private DiscussionFolder folder;
		private string title;

		public DiscussionTopic()
		{
		}

		public DiscussionTopic(DiscussionFolder folder, string name, string content, AgentLoginData agent)
		{
			Folder = folder;
			Name = name;
			Content = content;
			Author = agent.User;
			AuthorName = agent.Name;

			Created = DateTime.Now;
		}

		public virtual User Author { get; set; }

		public virtual string AuthorName
		{
			get => authorName;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				authorName = value;
			}
		}

		public virtual IList<DiscussionComment> Comments
		{
			get => comments;
			set
			{
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		public virtual string Content
		{
			get => content;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				content = value;
			}
		}

		public virtual DateTime Created { get; set; }

		public virtual bool Deleted { get; set; }

		public virtual EntryType EntryType => EntryType.DiscussionTopic;

		/// <summary>
		/// Folder containing this topic. Cannot be null.
		/// </summary>
		public virtual DiscussionFolder Folder
		{
			get => folder;
			set
			{
				ParamIs.NotNull(() => value);
				folder = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual bool Locked { get; set; }

		public virtual string Name
		{
			get => title;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				title = value;
			}
		}

		public virtual bool Pinned { get; set; }

		public virtual Comment CreateComment(string message, AgentLoginData loginData)
		{
			var comment = new DiscussionComment(this, message, loginData);
			Comments.Add(comment);
			return comment;
		}

		public virtual void MoveToFolder(DiscussionFolder targetFolder)
		{
			ParamIs.NotNull(() => targetFolder);

			if (targetFolder.Equals(Folder))
				return;

			Folder.AllTopics.Remove(this);
			Folder = targetFolder;
			Folder.AllTopics.Add(this);
		}

		public override string ToString()
		{
			return string.Format("Discussion topic '{0}' [{1}]", Name, Id);
		}
	}
}
