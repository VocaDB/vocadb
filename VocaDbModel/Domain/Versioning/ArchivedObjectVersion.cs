#nullable disable

using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Versioning;

public abstract class ArchivedObjectVersion : IDatabaseObject
{
#nullable enable
	private string _notes;
#nullable disable

	protected ArchivedObjectVersion()
	{
		Created = DateTime.Now;
	}

	protected ArchivedObjectVersion(XDocument data, AgentLoginData author, int version, EntryStatus status, string notes)
		: this()
	{
		ParamIs.NotNull(() => author);

		Data = data;
		AgentName = author.Name;
		Author = author.User;
		Notes = notes;
		Status = status;
		Version = version;
	}

#nullable enable
	public virtual string AgentName { get; protected set; }
#nullable disable

	/// <summary>
	/// User who created this version. Can be null.
	/// </summary>
	public virtual User Author { get; protected set; }

#nullable enable
	public virtual DateTime Created { get; protected set; }
#nullable disable

	/// <summary>
	/// Archived data serialized as XML.
	/// </summary>
	/// <remarks>For most types of entries, this will be non-null, but that is not guaranteed.</remarks>
	public virtual XDocument Data { get; protected set; }

	public abstract IEntryDiff DiffBase { get; }

#nullable enable
	public abstract EntryEditEvent EditEvent { get; }

	public abstract IEntryWithNames EntryBase { get; }

	public virtual bool Hidden { get; set; }

	public virtual int Id { get; protected set; }

	public virtual string Notes
	{
		get => _notes;
		[MemberNotNull(nameof(_notes))]
		protected set
		{
			ParamIs.NotNull(() => value);
			_notes = value;
		}
	}

	public virtual EntryStatus Status { get; protected set; }

	/// <summary>
	/// Version number. 
	/// </summary>
	/// <remarks>
	/// Note that not all entry types track version number. For those entry types this will always be 0.
	/// </remarks>
	public virtual int Version { get; protected set; }

	public override string ToString()
	{
		return $"archived version {Version} for {EntryBase}";
	}
#nullable disable
}
