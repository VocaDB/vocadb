using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts;

/// <summary>
/// Serializable data contract with (Int32) Id of the referred object
/// and the referred object's current name.
/// Used for serializing archived versions.
/// </summary>
[DataContract(Namespace = Schemas.VocaDb)]
public class ObjectRefContract : IEntryWithIntId
{
	[return: NotNullIfNotNull(nameof(entry))]
	public static ObjectRefContract? Create(IEntryBase? entry)
	{
		return entry != null ? new ObjectRefContract(entry) : null;
	}

#nullable disable
	public ObjectRefContract() { }
#nullable enable

	public ObjectRefContract(int id, string? nameHint)
	{
		Id = id;
		NameHint = nameHint;
	}

	public ObjectRefContract(IEntryBase entry)
	{
		ParamIs.NotNull(() => entry);

		Id = entry.Id;
		NameHint = entry.DefaultName;
	}

	/// <summary>
	/// Id of the referred object.
	/// </summary>
	[DataMember]
	public int Id { get; set; }

	[DataMember]
	public string? NameHint { get; init; }

	public override string ToString()
	{
		return $"{NameHint} [{Id}]";
	}
}
