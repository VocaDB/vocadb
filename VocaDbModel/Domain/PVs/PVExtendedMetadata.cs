using System.Runtime.Serialization;
using Newtonsoft.Json;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain.PVs
{

	/// <summary>
	/// Extended PV metadata serialized as JSON.
	/// This must be fully serializable and generally immutable.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class PVExtendedMetadata
	{

		public virtual T GetExtendedMetadata<T>() where T : class
		{

			if (string.IsNullOrEmpty(Json))
				return null;

			var obj = JsonConvert.DeserializeObject<T>(Json);
			return obj;

		}

		public PVExtendedMetadata() { }

		public PVExtendedMetadata(object data)
		{
			Json = data != null ? JsonConvert.SerializeObject(data) : null;
		}

		[DataMember]
		public string Json { get; set; }

	}

}
