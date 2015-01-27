using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.DataContracts.Discussions {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class DiscussionFolderContract {

		public DiscussionFolderContract() { }

		public DiscussionFolderContract(DiscussionFolder folder) {

			ParamIs.NotNull(() => folder);

			this.Description = folder.Description;
			this.Id = folder.Id;
			this.Name = folder.Name;
			
			// TODO
			this.LastTopicDate = folder.Topics.Any() ? (DateTime?)folder.Topics.Max(t => t.CreateDate) : null; 
			this.TopicCount = folder.Topics.Count;

		}

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public DateTime? LastTopicDate { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int TopicCount { get; set; }

	}

}
