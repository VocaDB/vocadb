using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.PVs;

[DataContract(Namespace = Schemas.VocaDb)]
public class ArchivedPVContract
{
#nullable disable
	public ArchivedPVContract()
	{
		Author = ThumbUrl = string.Empty;
	}
#nullable enable

	public ArchivedPVContract(PV pv)
		: this()
	{
		ParamIs.NotNull(() => pv);

		Author = pv.Author;
		ExtendedMetadata = pv.ExtendedMetadata;
		Name = pv.Name;
		PVId = pv.PVId;
		Service = pv.Service;
		PVType = pv.PVType;
	}

	public ArchivedPVContract(PVForSong pv)
		: this((PV)pv)
	{
		Disabled = pv.Disabled;
		Length = pv.Length;
		PublishDate = pv.PublishDate;
		ThumbUrl = pv.ThumbUrl;
	}

	[DataMember]
	public string Author { get; init; }

	[DataMember]
	public bool Disabled { get; init; }

	[DataMember]
	public PVExtendedMetadata? ExtendedMetadata { get; init; }

	[DataMember]
	public int Length { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public DateTime? PublishDate { get; init; }

	[DataMember]
	public string PVId { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public PVService Service { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public PVType PVType { get; init; }

	[DataMember]
	public string ThumbUrl { get; init; }
}
