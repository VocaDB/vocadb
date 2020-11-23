using System.Runtime.Serialization;

namespace VocaDb.SiteMapGenerator.VocaDb.DataContracts
{

	[DataContract]
	public class PartialFindResult<T>
	{

		[DataMember]
		public T[] Items { get; set; }

	}
}
