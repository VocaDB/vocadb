using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Web.Models.Shared.Partials.ArchivedEntry;

namespace VocaDb.Web.Helpers
{
	public static class ArchivedEntryHtmlExtensions
	{
		public static IHtmlString DataRow<T>(this HtmlHelper htmlHelper, string name, T primary, T compared, Func<T, object> valGetter, bool preserveLineBreaks = false)
			where T : class
		{

			var val1 = valGetter(primary);
			var val2 = (compared != null ? valGetter(compared) : null);

			return htmlHelper.Partial("Partials/ArchivedEntry/_DataRow", new DataRowViewModel(name, val1, val2, preserveLineBreaks));

		}

		public static IHtmlString DataRow<T>(this HtmlHelper htmlHelper, string name, ComparedVersionsContract<T> comparedVersions, Func<T, object> valGetter, bool preserveLineBreaks = false)
			where T : class
		{

			var val1 = valGetter(comparedVersions.FirstData);
			var val2 = (comparedVersions.SecondData != null ? valGetter(comparedVersions.SecondData) : null);

			return htmlHelper.Partial("Partials/ArchivedEntry/_DataRow", new DataRowViewModel(name, val1, val2, preserveLineBreaks));

		}

		public static IHtmlString DataRowList<T>(this HtmlHelper htmlHelper, string name, T primary, T compared, Func<T, IEnumerable<IHtmlString>> valGetter)
			where T : class
		{

			var val1 = valGetter(primary);
			var val2 = (compared != null ? valGetter(compared) : null);

			return htmlHelper.Partial("Partials/ArchivedEntry/_DataRowList", new DataRowListViewModel(name, val1, val2));

		}

		public static IHtmlString DataRowList<T>(this HtmlHelper htmlHelper, string name, ComparedVersionsContract<T> comparedVersions, Func<T, IEnumerable<IHtmlString>> valGetter)
			where T : class
		{

			var val1 = valGetter(comparedVersions.FirstData);
			var val2 = (comparedVersions.SecondData != null ? valGetter(comparedVersions.SecondData) : null);

			return htmlHelper.Partial("Partials/ArchivedEntry/_DataRowList", new DataRowListViewModel(name, val1, val2));

		}

		public static string FormatReleaseDate(OptionalDateTimeContract contract)
		{
			return OptionalDateTime.ToDateTime(contract.Year, contract.Month, contract.Day).ToShortDateString();
		}

		public static IHtmlString ObjectRefList<T>(this HtmlHelper htmlHelper, string name, ComparedVersionsContract<T> comparedVersions,
			Func<T, IEnumerable<ObjectRefContract>> valGetter) where T : class
		{

			return DataRowList(htmlHelper, name, comparedVersions, d => DataFormatUtils.GenerateHtml(valGetter(d), objRef => htmlHelper.Partial("Partials/ArchivedEntry/_ObjectRefInfo", new ObjectRefInfoViewModel(objRef))));

		}

		public static IHtmlString PictureRow<T>(this HtmlHelper htmlHelper, string name, ComparedVersionsContract<T> comparedVersions, Func<int, string> urlGetter)
			where T : class
		{

			var val1 = urlGetter(comparedVersions.FirstId);
			var val2 = (comparedVersions.SecondId != 0 ? urlGetter(comparedVersions.SecondId) : null);

			return htmlHelper.Partial("Partials/ArchivedEntry/_PictureRow", new PictureRowViewModel(name, val1, val2));

		}

		public static IHtmlString TranslatedNameRow<T>(this HtmlHelper htmlHelper, ComparedVersionsContract<T> comparedVersions, Func<T, ITranslatedString> valGetter)
			where T : class
		{

			var val1 = valGetter(comparedVersions.FirstData);
			var val2 = comparedVersions.SecondData != null ? valGetter(comparedVersions.SecondData) : null;

			return htmlHelper.Partial("Partials/ArchivedEntry/_TranslatedNameRow", new TranslatedNameRowViewModel(val1, val2));

		}
	}
}