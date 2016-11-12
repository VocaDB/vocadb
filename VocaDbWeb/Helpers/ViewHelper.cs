using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using System.Collections.Generic;
using System.Web;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Translations;
using VocaDb.Model.Utils;
using VocaDb.Web.Code.Markdown;

namespace VocaDb.Web.Helpers {

	public static class ViewHelper {

		private static Dictionary<ContentLanguageSelection, string> LanguageSelections {
			get {

				return EnumVal<ContentLanguageSelection>.Values
					.ToDictionary(l => l, Translate.ContentLanguageSelectionName);

			}

		}

		private static Dictionary<ContentLanguageSelection, string> LanguageSelectionsWithoutUnspecified {
			get {

				return EnumVal<ContentLanguageSelection>.Values.Where(l => l != ContentLanguageSelection.Unspecified)
					.ToDictionary(l => l, Translate.ContentLanguageSelectionName);

			}

		}

		private static Dictionary<ContentLanguagePreference, string> LanguagePreferences {
			get {

				return EnumVal<ContentLanguagePreference>.Values
					.ToDictionary(l => l, Translate.ContentLanguagePreferenceName);

			}
		}

		public static SelectList LanguagePreferenceList {
			get {
				return new SelectList(LanguagePreferences, "Key", "Value");
			}
		}

		public static IHtmlString ConditionalAttribute(this HtmlHelper html, bool condition, string attributeName, string value) {
			
			if (condition) {
				return new MvcHtmlString(!string.IsNullOrEmpty(value) ? attributeName + "=\"" + html.Encode(value) + "\"" : attributeName);
			}

			return MvcHtmlString.Empty;

		}

		public static SelectList CreateArtistTypesList(object selectedValue) {
			return new SelectList(AppConfig.ArtistTypes.ToDictionary(s => s, Translate.ArtistTypeName), "Key", "Value", selectedValue);
		}

		public static SelectList CreateDiscTypesList(object selectedValue) {
			return new SelectList(EnumVal<DiscType>.Values.ToDictionary(s => s, Translate.DiscTypeName), "Key", "Value", selectedValue);
		}

		public static SelectList CreateEmailOptionsList(object selectedValue) {
			return new SelectList(EnumVal<UserEmailOptions>.Values.ToDictionary(s => s, Translate.EmailOptions), "Key", "Value", selectedValue);
		}

		public static SelectList CreateEnumList<T>(object selectedValue, TranslateableEnum<T> enumType) where T : struct, IConvertible {
			return CreateEnumList(selectedValue, enumType.AllFields);
		}

		public static SelectList CreateEnumList<T>(object selectedValue, IEnumerable<KeyValuePair<T, string>> vals) where T : struct, IConvertible {
			return new SelectList(vals, "Key", "Value", selectedValue);
		}

		public static SelectList CreateEnumList<T>(object selectedValue, IEnumerable<TranslateableEnumField<T>> vals) where T : struct, IConvertible {
			return new SelectList(vals, "Id", "Name", selectedValue);
		}

		public static SelectList CreateLanguageSelectionList(object selectedValue) {
			return new SelectList(LanguageSelections, "Key", "Value", selectedValue);
		}

		public static SelectList CreateLanguageSelectionListWithoutUnspecified(object selectedValue) {
			return new SelectList(LanguageSelectionsWithoutUnspecified, "Key", "Value", selectedValue);
		}

		public static SelectList CreateSelectList<T>(IEnumerable<T> items, Func<T, object> keyFactory, Func<T, object> labelFactory, object selectedValue) {
			return new SelectList(items.Select(i => new{
				Key = keyFactory(i).ToString(),
				Label = labelFactory(i).ToString()
			}), "Key", "Label", selectedValue);
		}

		public static SelectList CreateSongTypesList(object selectedValue) {
			return new SelectList(AppConfig.SongTypes.ToDictionary(s => s, Translate.SongTypeNames.GetName), "Key", "Value", selectedValue);
		}

		public static MvcHtmlString ArtistTypeDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ArtistType>> expression, object htmlAttributes = null, object selectedValue = null) {

			return htmlHelper.DropDownListFor(expression, CreateArtistTypesList(selectedValue), htmlAttributes);

		}

		public static MvcHtmlString DiscTypeDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, DiscType>> expression, object htmlAttributes = null, object selectedValue = null) {

			return htmlHelper.DropDownListFor(expression, CreateDiscTypesList(selectedValue), htmlAttributes);

		}

		public static MvcHtmlString EmailOptionsDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, UserEmailOptions>> expression, object htmlAttributes = null, object selectedValue = null) {

			return htmlHelper.DropDownListFor(expression, CreateEmailOptionsList(selectedValue), htmlAttributes);

		}

		public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name,
			TranslateableEnum<TEnum> enumType, object htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, IConvertible {

			return htmlHelper.DropDownList(name, CreateEnumList(selectedValue, enumType), htmlAttributes);

		}

		public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name,
			IEnumerable<TranslateableEnumField<TEnum>> enumType, object htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, IConvertible {

			return htmlHelper.DropDownList(name, CreateEnumList(selectedValue, enumType), htmlAttributes);

		}

		public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, 
			Expression<Func<TModel, TEnum>> expression,
			TranslateableEnum<TEnum> enumType, object htmlAttributes = null, object selectedValue = null) 
			where TEnum : struct, IConvertible {

			return htmlHelper.DropDownListFor(expression, CreateEnumList(selectedValue, enumType), htmlAttributes);

		}

		public static MvcHtmlString EnumDropDownListForDic<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TEnum>> expression,
			TranslateableEnum<TEnum> enumType, IDictionary<string, object> htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, IConvertible {

			return htmlHelper.DropDownListFor(expression, CreateEnumList(selectedValue, enumType), htmlAttributes);

		}

		public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TEnum>> expression,
			IEnumerable<KeyValuePair<TEnum, string>> values, object htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, IConvertible {

			return htmlHelper.DropDownListFor(expression, CreateEnumList(selectedValue, values), htmlAttributes);

		}

		public static string EntryThumbUrl(this UrlHelper url, IEntryImageInformation imageInfo, string songThumbUrl) {

			switch (imageInfo.EntryType) {
				case EntryType.Album:
					return url.ImageThumb(imageInfo, ImageSize.TinyThumb);
				case EntryType.Artist:
					return url.ImageThumb(imageInfo, ImageSize.TinyThumb);
				case EntryType.Song:
					return songThumbUrl;
				case EntryType.SongList:
					return url.EntryImageOld(imageInfo, ImageSize.SmallThumb);
				case EntryType.Tag:
					return url.EntryImageOld(imageInfo, ImageSize.SmallThumb);
				default:
					return string.Empty;
			}

		}

		public static int GetComparedEntryId(ArchivedObjectVersionContract archivedVersion, int comparedEntryId, 
			IEnumerable<ArchivedObjectVersionContract> allVersions) {

			if (comparedEntryId != 0)
				return comparedEntryId;

			var nextVersion = allVersions.FirstOrDefault(v => v.Version == archivedVersion.Version - 1);

			return (nextVersion ?? archivedVersion).Id;

		}

		public static object GetRouteParams(SongContract contract, int? albumId = null) {
			return new { id = contract.Id, albumId };
			//return new { id = contract.Id, friendlyName = VocaUrlHelper.GetUrlFriendlyName(contract.TranslatedName) };
		}

		/// <summary>
		/// Prints a number of items in a grid as HTML table
		/// </summary>
		/// <typeparam name="T">Type of items to be processed.</typeparam>
		/// <param name="htmlHelper">HTML helper. Cannot be null.</param>
		/// <param name="items">List of items to be printed. Cannot be null or empty.</param>
		/// <param name="columns">Number of  columns in the grid. Must be higher than 0.</param>
		/// <param name="contentFunc">
		/// Provides the content for individual table cells. Return value is raw HTML. Cannot be null.</param>
		/// <returns></returns>
		public static MvcHtmlString Grid<T>(this HtmlHelper htmlHelper, IEnumerable<T> items, 
			int columns, Func<T, IHtmlString> contentFunc) {

			ParamIs.NotNull(() => htmlHelper);
			ParamIs.NotNull(() => items);
			ParamIs.Positive(columns, "columns");
			ParamIs.NotNull(() => contentFunc);

			var tableTag = new TagBuilder("table");
			TagBuilder trTag = null;
			int i = 0;

			foreach (var item in items) {

				if (i % columns == 0) {

					if (trTag != null)
						tableTag.InnerHtml += trTag.ToString();

					trTag = new TagBuilder("tr");

				}

				var tdTag = new TagBuilder("td");
				tdTag.InnerHtml = contentFunc(item).ToHtmlString();
				trTag.InnerHtml += tdTag.ToString();
				i++;

			}

			if (trTag != null)
				tableTag.InnerHtml += trTag.ToString();

			return new MvcHtmlString(tableTag.ToString());

		}

		public static MvcHtmlString LanguagePreferenceDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ContentLanguagePreference>> expression) {

			return htmlHelper.DropDownListFor(expression, LanguagePreferenceList);

		}

		public static MvcHtmlString LanguageSelectionDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ContentLanguageSelection>> expression, object htmlAttributes = null, bool allowUnspecified = false, object selectedValue = null) {

			return htmlHelper.DropDownListFor(expression, allowUnspecified ? CreateLanguageSelectionList(selectedValue) : CreateLanguageSelectionListWithoutUnspecified(selectedValue), htmlAttributes);

		}

		public static MvcHtmlString LinkList<T>(this HtmlHelper htmlHelper, IEnumerable<T> list, Func<T, MvcHtmlString> linkFunc) {

			return StringHelper.Join(", ", list.Select(linkFunc));

		}

		public static MvcHtmlString LinkListHtml<T>(this HtmlHelper htmlHelper, IEnumerable<T> list, Func<T, IHtmlString> linkFunc) {

			return StringHelper.Join(", ", list.Select(linkFunc));

		}

		public static MvcHtmlString SongTypeDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, SongType>> expression, object htmlAttributes = null, object selectedValue = null) {

			return htmlHelper.DropDownListFor(expression, CreateSongTypesList(selectedValue), htmlAttributes);

		}

		public static MvcHtmlString FormatMarkdown(this HtmlHelper htmlHelper, string markdown) {
			return new MvcHtmlString(DependencyResolver.Current.GetService<MarkdownParser>().GetHtml(markdown));
		}

		public static string VideoServiceLinkUrl(this HtmlHelper htmlHelper, PVService service) {

			switch (service) {
				case PVService.Bilibili:
					return UrlHelper.GenerateContentUrl("~/Content/ExtIcons/bilibili.png", new HttpContextWrapper(HttpContext.Current));
				case PVService.File:
				case PVService.LocalFile:
					return UrlHelper.GenerateContentUrl("~/Content/Icons/music.png", new HttpContextWrapper(HttpContext.Current));
				case PVService.NicoNicoDouga:
					return UrlHelper.GenerateContentUrl("~/Content/nico.png", new HttpContextWrapper(HttpContext.Current));
				case PVService.Piapro:
					return UrlHelper.GenerateContentUrl("~/Content/ExtIcons/piapro.png", new HttpContextWrapper(HttpContext.Current));
				case PVService.SoundCloud:
					return UrlHelper.GenerateContentUrl("~/Content/Icons/soundcloud.png", new HttpContextWrapper(HttpContext.Current));
				case PVService.Youtube:
					return UrlHelper.GenerateContentUrl("~/Content/youtube.png", new HttpContextWrapper(HttpContext.Current));
				case PVService.Vimeo:
					return UrlHelper.GenerateContentUrl("~/Content/ExtIcons/vimeo.png", new HttpContextWrapper(HttpContext.Current));
				default:
					return string.Empty;
			}

		}

	}

}