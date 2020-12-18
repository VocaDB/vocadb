#nullable disable

using System;
using System.Globalization;

namespace VocaDb.Model.Domain.Globalization
{
	public class OptionalCultureCode : IEquatable<OptionalCultureCode>
	{
		public static OptionalCultureCode Empty => new OptionalCultureCode(string.Empty);
		public const string LanguageCode_English = "en";
		public const string LanguageCode_Japanese = "ja";

		public static CultureInfo GetCultureInfo(string cultureCode)
		{
			return !string.IsNullOrEmpty(cultureCode) ? CultureInfo.GetCultureInfo(cultureCode) : null;
		}

		private string cultureCode;

		public OptionalCultureCode() : this(string.Empty) { }

		public OptionalCultureCode(string cultureCode)
		{
			CultureCode = cultureCode;
		}

		public OptionalCultureCode(CultureInfo culture, bool onlyLanguage)
		{
			CultureCode = onlyLanguage ? culture?.TwoLetterISOLanguageName : culture?.Name;
		}

		/// <summary>
		/// .NET culture associated with this code. Can be null.
		/// </summary>
		public virtual CultureInfo CultureInfo => GetCultureInfo(CultureCode);

		public virtual string CultureCode
		{
			get => cultureCode;
			set => cultureCode = value ?? string.Empty;
		}

		public virtual bool IsEmpty => string.IsNullOrEmpty(CultureCode);

		public override string ToString()
		{
			return CultureCode;
		}

		public virtual bool Equals(OptionalCultureCode culture)
		{
			return string.Equals(CultureCode, culture?.CultureCode ?? string.Empty, StringComparison.InvariantCultureIgnoreCase);
		}

		public virtual bool Equals(CultureInfo culture)
		{
			return string.Equals(CultureCode, culture?.Name ?? string.Empty, StringComparison.InvariantCultureIgnoreCase);
		}

		public virtual bool Equals(string cultureCode)
		{
			return string.Equals(CultureCode, cultureCode ?? string.Empty, StringComparison.InvariantCultureIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as OptionalCultureCode);
		}

		public virtual CultureInfo GetCultureInfoSafe()
		{
			if (IsEmpty)
				return null;

			try
			{
				return CultureInfo;
			}
			catch (CultureNotFoundException)
			{
				return null;
			}
		}

		public override int GetHashCode()
		{
			return !IsEmpty ? CultureCode.GetHashCode() : 0;
		}
	}
}
