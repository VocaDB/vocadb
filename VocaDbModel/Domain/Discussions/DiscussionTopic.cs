#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
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

		private string _authorName;
		private IList<DiscussionComment> _comments = new List<DiscussionComment>();
		private DiscussionFolder _folder;
		private string _title;

		public DiscussionTopic()
		{
		}

		public DiscussionTopic(DiscussionFolder folder, string name, string content, AgentLoginData agent)
		{
			Folder = folder;
			Name = name;
			CreateComment(content, agent);
			AuthorName = agent.Name;

			Created = DateTime.Now;
		}

		public virtual User Author => FirstComment.Author;

		public virtual string AuthorName
		{
			get => _authorName;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				_authorName = value;
			}
		}

		public virtual IList<DiscussionComment> AllComments
		{
			get => _comments;
			set
			{
				ParamIs.NotNull(() => value);
				_comments = value;
			}
		}

		/// <remarks>
		/// The <see cref="FirstComment"/> is regarded as the content of a topic, therefore, we exclude it from <see cref="Comments"/>.
		/// </remarks>
		public virtual IEnumerable<DiscussionComment> Comments => AllComments.Where(c => c != FirstComment).Where(c => !c.Deleted);

		public virtual string Content => FirstComment.Message;

		public virtual DateTime Created { get; set; }

		public virtual bool Deleted { get; set; }

		public virtual EntryType EntryType => EntryType.DiscussionTopic;

		public virtual DiscussionComment FirstComment => AllComments.OrderBy(c => c.Created).First();

		/// <summary>
		/// Folder containing this topic. Cannot be null.
		/// </summary>
		public virtual DiscussionFolder Folder
		{
			get => _folder;
			set
			{
				ParamIs.NotNull(() => value);
				_folder = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual bool Locked { get; set; }

		public virtual string Name
		{
			get => _title;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				_title = value;
			}
		}

		public virtual bool Pinned { get; set; }

		public virtual Comment CreateComment(string message, AgentLoginData loginData)
		{
			var comment = new DiscussionComment(this, message, loginData);
			AllComments.Add(comment);
			return comment;
		}

		public virtual void Delete()
		{
			Deleted = true;
			FirstComment.Deleted = true;
		}

#nullable enable
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
			return $"Discussion topic '{Name}' [{Id}]";
		}
#nullable disable
	}
}
