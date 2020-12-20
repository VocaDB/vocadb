#nullable disable

using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Web.Models.Shared
{
	public class EnglishTranslatedStringViewModel
	{
		public EnglishTranslatedStringViewModel(EnglishTranslatedString str, int maxLength = 500, int summaryLength = 400)
		{
			String = str;
			MaxLength = maxLength;
			SummaryLength = summaryLength;
		}

		public EnglishTranslatedString String { get; }

		/// <summary>
		/// Maximum length of string before it is shortened.
		/// Generally this should be slightly more than SummaryLength.
		/// </summary>
		public int MaxLength { get; set; }

		/// <summary>
		/// Length of the summary (shortened) text.
		/// Generally this should be the same or smaller than MaxLength.
		/// </summary>
		public int SummaryLength { get; set; }
	}
}