#nullable disable

using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Globalization;

public class LocalizedStringWithId : LocalizedString, IEntryWithIntId
{
	public LocalizedStringWithId() { }

	public LocalizedStringWithId(string val, ContentLanguageSelection language)
		: base(val, language) { }

	public virtual int Id { get; set; }

	public override string Value
	{
		get => base.Value;
		set
		{
			base.Value = value;
			SearchValue = NameSearchHelper.Normalize(value);
		}
	}

	/// <summary>
	/// <see cref="Value"/> folded for substring search (see <see cref="NameSearchHelper"/>).
	/// </summary>
	public virtual string SearchValue { get; protected set; }
}
