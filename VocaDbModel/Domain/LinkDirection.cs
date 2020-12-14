#nullable disable

namespace VocaDb.Model.Domain
{
	/// <summary>
	/// Direction of a many-to-one or many-to-many bidirectional link between entities (usually child-parent relationship).
	/// </summary>
	public enum LinkDirection
	{
		/// <summary>
		/// From child to parent (default).
		/// </summary>
		ManyToOne,

		/// <summary>
		/// From parent to child (reverse link).
		/// </summary>
		OneToMany
	}
}
