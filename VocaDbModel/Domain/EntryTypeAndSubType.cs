#nullable disable

using System;

namespace VocaDb.Model.Domain
{
	/// <summary>
	/// Combination of <see cref="EntryType"/> and optional sub-type (for example SongType).
	/// </summary>
	public readonly struct EntryTypeAndSubType
	{
		public static EntryTypeAndSubType Create<TSubType>(EntryType entryType, TSubType subType)
			where TSubType : struct, Enum
		{
			return new EntryTypeAndSubType(entryType, subType.ToString());
		}

		public EntryTypeAndSubType(EntryType entryType, string subType = "")
		{
			EntryType = entryType;
			SubType = subType ?? string.Empty;
		}

		public EntryType EntryType { get; }
		public bool HasValue => EntryType != EntryType.Undefined;
		public bool HasSubType => !string.IsNullOrEmpty(SubType);

		/// <summary>
		/// Sub-type, for example "Remix".
		/// This value is from one of the sub-type enums (ArtistType, SongType etc.).
		/// Can be empty, if not specified.
		/// </summary>
		public string SubType { get; }
	}
}
