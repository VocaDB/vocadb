using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts
{

	/// <summary>
	/// Entry name (title) with primary display name and additional names.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryNameContract
	{

		public static EntryNameContract Empty
		{
			get
			{
				return new EntryNameContract(string.Empty);
			}
		}

		public EntryNameContract(string displayName, string additionalNames)
		{
			DisplayName = displayName;
			AdditionalNames = additionalNames;
		}

		public EntryNameContract(string displayName)
			: this(displayName, string.Empty) { }

		/// <summary>
		/// Comma-separated list of additional names (excluding the primary display name).
		/// </summary>
		[DataMember]
		public string AdditionalNames { get; set; }

		/// <summary>
		/// Display name is the primary name localized to the requested language.
		/// </summary>
		[DataMember]
		public string DisplayName { get; set; }

	}

}
