#nullable disable

using System;

namespace VocaDb.Model.Domain.Tags
{
	/// <summary>
	/// Defines tag mapping from an external source system such as NicoNicoDouga to VocaDB.
	/// </summary>
	public class TagMapping : IEntryWithIntId, ITagLink
	{
		public TagMapping()
		{
			CreateDate = DateTime.Now;
			MappingType = TagMappingType.Automatic;
		}

#nullable enable
		public TagMapping(Tag tag, string sourceTag) : this()
		{
			ParamIs.NotNull(() => tag);
			ParamIs.NotNullOrEmpty(() => sourceTag);

			Tag = tag;
			SourceTag = sourceTag;
		}
#nullable disable

		private Tag _tag;
		private string _sourceTag;

		public virtual DateTime CreateDate { get; set; }

		public virtual int Id { get; set; }

		public virtual TagMappingType MappingType { get; set; }

		/// <summary>
		/// Tag name in the source system.
		/// For example "VOCAROCK".
		/// </summary>
		public virtual string SourceTag
		{
			get => _sourceTag;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				_sourceTag = value;
			}
		}

		/// <summary>
		/// VocaDB tag. Cannot be null.
		/// </summary>
		public virtual Tag Tag
		{
			get => _tag;
			set
			{
				ParamIs.NotNull(() => value);
				_tag = value;
			}
		}

		public virtual void Delete()
		{
			Tag.Mappings.Remove(this);
		}

#nullable enable
		public override string ToString() => $"'{SourceTag}' => '{Tag.DefaultName}'";
#nullable disable
	}

	public enum TagMappingType
	{
		Nothing = 0,
		/// <summary>
		/// Tag appears in related tags, but is not automatically mapped
		/// </summary>
		Related = 1,
		/// <summary>
		/// Tag is automatically applied when adding song
		/// </summary>
		Automatic = 2,
		/// <summary>
		/// Tag is only offered as suggestion
		/// </summary>
		Suggestion = 4
	}
}
