using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumDiffContract {

		public AlbumDiffContract() { }

		public AlbumDiffContract(AlbumDiff diff) { 
		
			ParamIs.NotNull(() => diff);

			Cover = diff.Cover;
			Description = diff.Description;
			IsSnapshot = diff.IsSnapshot;
			Names = diff.Names;
			WebLinks = diff.WebLinks;
		
		}

		[DataMember]
		public bool Cover { get; set; }

		[DataMember]
		public bool Description { get; set; }

		[DataMember]
		public bool IsSnapshot { get; set; }

		[DataMember]
		public bool Names { get; set; }

		[DataMember]
		public bool WebLinks { get; set; }

	}
}
