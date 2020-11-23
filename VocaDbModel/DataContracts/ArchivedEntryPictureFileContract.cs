using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts
{

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedEntryPictureFileContract
	{

		public ArchivedEntryPictureFileContract() { }

		public ArchivedEntryPictureFileContract(EntryPictureFile entryPictureFile)
		{

			ParamIs.NotNull(() => entryPictureFile);

			Author = new ObjectRefContract(entryPictureFile.Author);
			Created = entryPictureFile.Created;
			Id = entryPictureFile.Id;
			Mime = entryPictureFile.Mime;
			Name = entryPictureFile.Name;

		}

		[DataMember]
		public ObjectRefContract Author { get; set; }

		[DataMember]
		public DateTime Created { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Mime { get; set; }

		[DataMember]
		public string Name { get; set; }

	}

}
