#nullable disable

namespace VocaDb.Model.Domain.Globalization
{
	/// <summary>
	/// Interface for localized strings (string with a language selection).
	/// </summary>
	public interface ILocalizedString
	{
		ContentLanguageSelection Language { get; }

		string Value { get; }
	}
}
