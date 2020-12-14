#nullable disable

using System.Runtime.Serialization;

namespace VocaDb.SiteMapGenerator.VocaDb
{
	[DataContract]
	public class EntryIdContract
	{
		[DataMember]
		public int Id { get; set; }
	}
}
