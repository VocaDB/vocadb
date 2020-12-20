#nullable disable

using System;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using System.Collections.Generic;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Translations;
using VocaDb.Model.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Routing;

namespace VocaDb.Web.Helpers
{
	// TODO: implement
	public static class ViewHelper
	{
		private static Dictionary<ContentLanguageSelection, string> LanguageSelections
		{
			get
			{
				return EnumVal<ContentLanguageSelection>.Values
					.ToDictionary(l => l, Translate.ContentLanguageSelectionName);
			}
		}

		private static Dictionary<ContentLanguageSelection, string> LanguageSelectionsWithoutUnspecified
		{
			get
			{
				return EnumVal<ContentLanguageSelection>.Values.Where(l => l != ContentLanguageSelection.Unspecified)
					.ToDictionary(l => l, Translate.ContentLanguageSelectionName);
			}
		}

		private static Dictionary<ContentLanguagePreference, string> LanguagePreferences
		{
			get
			{
				return EnumVal<ContentLanguagePreference>.Values
					.ToDictionary(l => l, Translate.ContentLanguagePreferenceName);
			}
		}

		public static SelectList LanguagePreferenceList => new SelectList(LanguagePreferences, "Key", "Value");

		public static IHtmlContent ConditionalAttribute(this IHtmlHelper html, bool condition, string attributeName, string value)
		{
			if (condition)
			{
				return new HtmlString(!string.IsNullOrEmpty(value) ? attributeName + "=\"" + html.Encode(value) + "\"" : attributeName);
			}

			return HtmlString.Empty;
		}

		public static SelectList CreateArtistTypesList(object selectedValue)
		{
			return new SelectList(AppConfig.ArtistTypes.ToDictionary(s => s, Translate.ArtistTypeName), "Key", "Value", selectedValue);
		}

		public static SelectList CreateDiscTypesList(object selectedValue)
		{
			return new SelectList(AppConfig.AlbumTypes.ToDictionary(s => s, Translate.DiscTypeName), "Key", "Value", selectedValue);
		}

		public static SelectList CreateEmailOptionsList(object selectedValue)
		{
			return new SelectList(EnumVal<UserEmailOptions>.Values.ToDictionary(s => s, Translate.EmailOptions), "Key", "Value", selectedValue);
		}

		public static SelectList CreateEnumList<T>(object selectedValue, TranslateableEnum<T> enumType) where T : struct, Enum
		{
			return CreateEnumList(selectedValue, enumType.AllFields);
		}

		public static SelectList CreateEnumList<T>(object selectedValue, IEnumerable<KeyValuePair<T, string>> vals) where T : struct, IConvertible
		{
			return new SelectList(vals, "Key", "Value", selectedValue);
		}

		public static SelectList CreateEnumList<T>(object selectedValue, IEnumerable<TranslateableEnumField<T>> vals) where T : struct, IConvertible
		{
			return new SelectList(vals, "Id", "Name", selectedValue);
		}

		public static SelectList CreateLanguageSelectionList(object selectedValue)
		{
			return new SelectList(LanguageSelections, "Key", "Value", selectedValue);
		}

		public static SelectList CreateLanguageSelectionListWithoutUnspecified(object selectedValue)
		{
			return new SelectList(LanguageSelectionsWithoutUnspecified, "Key", "Value", selectedValue);
		}

		public static SelectList CreateSelectList<T>(IEnumerable<T> items, Func<T, object> keyFactory, Func<T, object> labelFactory, object selectedValue)
		{
			return new SelectList(items.Select(i => new
			{
				Key = keyFactory(i).ToString(),
				Label = labelFactory(i).ToString()
			}), "Key", "Label", selectedValue);
		}

		public static SelectList CreateSongTypesList(object selectedValue)
		{
			return new SelectList(AppConfig.SongTypes.ToDictionary(s => s, Translate.SongTypeNames.GetName), "Key", "Value", selectedValue);
		}

		public static IHtmlContent ArtistTypeDropDownListFor<TModel>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ArtistType>> expression, object htmlAttributes = null, object selectedValue = null)
		{
			return htmlHelper.DropDownListFor(expression, CreateArtistTypesList(selectedValue), htmlAttributes);
		}

		public static IHtmlContent DiscTypeDropDownListFor<TModel>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, DiscType>> expression, object htmlAttributes = null, object selectedValue = null)
		{
			return htmlHelper.DropDownListFor(expression, CreateDiscTypesList(selectedValue), htmlAttributes);
		}

		public static IHtmlContent EmailOptionsDropDownListFor<TModel>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, UserEmailOptions>> expression, object htmlAttributes = null, object selectedValue = null)
		{
			return htmlHelper.DropDownListFor(expression, CreateEmailOptionsList(selectedValue), htmlAttributes);
		}

		public static IHtmlContent EnumDropDownList<TEnum>(this IHtmlHelper htmlHelper, string name,
			TranslateableEnum<TEnum> enumType, object htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, Enum
		{
			return htmlHelper.DropDownList(name, CreateEnumList(selectedValue, enumType), htmlAttributes);
		}

		public static IHtmlContent EnumDropDownList<TEnum>(this IHtmlHelper htmlHelper, string name,
			IEnumerable<TranslateableEnumField<TEnum>> enumType, object htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, Enum
		{
			return htmlHelper.DropDownList(name, CreateEnumList(selectedValue, enumType), htmlAttributes);
		}

		public static IHtmlContent EnumDropDownListFor<TModel, TEnum>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TEnum>> expression,
			TranslateableEnum<TEnum> enumType, object htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, Enum
		{
			return htmlHelper.DropDownListFor(expression, CreateEnumList(selectedValue, enumType), htmlAttributes);
		}

		public static IHtmlContent EnumDropDownListForDic<TModel, TEnum>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TEnum>> expression,
			TranslateableEnum<TEnum> enumType, IDictionary<string, object> htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, Enum
		{
			return htmlHelper.DropDownListFor(expression, CreateEnumList(selectedValue, enumType), htmlAttributes);
		}

		public static IHtmlContent EnumDropDownListFor<TModel, TEnum>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TEnum>> expression,
			IEnumerable<KeyValuePair<TEnum, string>> values, object htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, IConvertible
		{
			return htmlHelper.DropDownListFor(expression, CreateEnumList(selectedValue, values), htmlAttributes);
		}

		public static int GetComparedEntryId(ArchivedObjectVersionContract archivedVersion, int comparedEntryId,
			IEnumerable<ArchivedObjectVersionContract> allVersions)
		{
			if (comparedEntryId != 0)
				return comparedEntryId;

			var nextVersion = allVersions.FirstOrDefault(v => v.Version == archivedVersion.Version - 1);

			return (nextVersion ?? archivedVersion).Id;
		}

		public static object GetRouteParams(SongContract contract, int? albumId = null)
		{
			return new { id = contract.Id, albumId };
			//return new { id = contract.Id, friendlyName = VocaUrlHelper.GetUrlFriendlyName(contract.TranslatedName) };
		}

		public static RouteValueDictionary GetRouteValueDictionary(SongContract contract, int? albumId = null)
		{
			return new RouteValueDictionary { { "id", contract.Id }, { "albumId", albumId } };
		}

		// TODO: implement
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
		public static IHtmlContent Grid<T>(this IHtmlHelper htmlHelper, IEnumerable<T> items,
			int columns, Func<T, IHtmlContent> contentFunc) => throw new NotImplementedException();

		public static IHtmlContent LanguagePreferenceDropDownListFor<TModel>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ContentLanguagePreference>> expression)
		{
			return htmlHelper.DropDownListFor(expression, LanguagePreferenceList);
		}

		public static IHtmlContent LanguageSelectionDropDownListFor<TModel>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ContentLanguageSelection>> expression, object htmlAttributes = null, bool allowUnspecified = false, object selectedValue = null)
		{
			return htmlHelper.DropDownListFor(expression, allowUnspecified ? CreateLanguageSelectionList(selectedValue) : CreateLanguageSelectionListWithoutUnspecified(selectedValue), htmlAttributes);
		}

		public static IHtmlContent LinkList<T>(this IHtmlHelper htmlHelper, IEnumerable<T> list, Func<T, IHtmlContent> linkFunc)
		{
			return StringHelper.Join(", ", list.Select(linkFunc));
		}

		public static IHtmlContent LinkListHtml<T>(this IHtmlHelper htmlHelper, IEnumerable<T> list, Func<T, IHtmlContent> linkFunc)
		{
			return StringHelper.Join(", ", list.Select(linkFunc));
		}

		public static IHtmlContent SongTypeDropDownListFor<TModel>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, SongType>> expression, object htmlAttributes = null, object selectedValue = null)
		{
			return htmlHelper.DropDownListFor(expression, CreateSongTypesList(selectedValue), htmlAttributes);
		}

		// TODO: implement
		public static IHtmlContent FormatMarkdown(this IHtmlHelper htmlHelper, string markdown) => throw new NotImplementedException();

		// TODO: implement
		public static string StripMarkdown(this IHtmlHelper htmlHelper, string markdown) => throw new NotImplementedException();

		// TODO: implement
		public static string VideoServiceLinkUrl(this IHtmlHelper htmlHelper, PVService service) => throw new NotImplementedException();
	}
}