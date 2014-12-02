using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using VocaDb.Model;

namespace VocaDb.Web.Helpers {

	/// <summary>
	/// Extends <see cref="HtmlHelper"/> with Knockout-specific methods.
	/// </summary>
	public static class KnockoutHtmlExtender {

		/// <summary>
		/// Dropdown list bound to knockout model.
		/// </summary>
		public static MvcHtmlString DropdownForKnockout<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
			SelectList selectList,
			string binding, string cssClass = null, string id = null) {

			ParamIs.NotNull(() => htmlHelper);
			ParamIs.NotNull(() => expression);
			ParamIs.NotNull(() => binding);

			var htmlAttributes = new Dictionary<string, object> { { "data-bind", binding } };

			if (!string.IsNullOrEmpty(cssClass))
				htmlAttributes.Add("class", cssClass);

			if (!string.IsNullOrEmpty(id))
				htmlAttributes.Add("id", id);

			return htmlHelper.DropDownListFor(expression, selectList, htmlAttributes);

		}

		public static MvcHtmlString HiddenForKnockout<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
			string binding, string id = null) {

			ParamIs.NotNull(() => htmlHelper);
			ParamIs.NotNull(() => expression);
			ParamIs.NotNull(() => binding);

			var htmlAttributes = new Dictionary<string, object> { { "data-bind", binding } };

			if (!string.IsNullOrEmpty(id))
				htmlAttributes.Add("id", id);

			return htmlHelper.HiddenFor(expression, htmlAttributes);

		}

		/// <summary>
		/// Text area bound to knockout model.
		/// </summary>
		/// <typeparam name="TModel">Model type.</typeparam>
		/// <typeparam name="TProperty">Property expression type.</typeparam>
		/// <param name="htmlHelper">HTML helper. Cannot be null.</param>
		/// <param name="expression">Property access expression. Cannot be null.</param>
		/// <param name="binding">Knockout data-bind attribute. Cannot be null.</param>
		/// <param name="cssClass">CSS class attribute. Can be null or empty, in which case this attribute is not specified.</param>
		/// <param name="id">ID attribute. Can be null or empty, in which case this attribute is not specified.</param>
		/// <param name="maxLength">Max length attribute. Can be null, in which case this attribute is not specified.</param>
		/// <param name="rows">Rows attribute. Can be null, in which case this attribute is not specified.</param>
		/// <param name="cols">Cols attribute. Can be null, in which case this attribute is not specified.</param>
		/// <returns></returns>
		public static MvcHtmlString TextAreaForKnockout<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, 
			string binding, string cssClass = null, string id = null, int? maxLength = null, int? rows = null, int? cols = null) {

			ParamIs.NotNull(() => htmlHelper);
			ParamIs.NotNull(() => expression);
			ParamIs.NotNull(() => binding);

			var htmlAttributes = new Dictionary<string, object> { { "data-bind", binding } };

			if (!string.IsNullOrEmpty(cssClass))
				htmlAttributes.Add("class", cssClass);

			if (!string.IsNullOrEmpty(id))
				htmlAttributes.Add("id", id);

			if (maxLength.HasValue)
				htmlAttributes.Add("maxlength", maxLength.Value);

			if (rows.HasValue)
				htmlAttributes.Add("rows", rows.Value);

			if (cols.HasValue)
				htmlAttributes.Add("cols", cols.Value);

			return htmlHelper.TextAreaFor(expression, htmlAttributes);

		}

		/// <summary>
		/// Text box bound to knockout model.
		/// </summary>
		/// <typeparam name="TModel">Model type.</typeparam>
		/// <typeparam name="TProperty">Property expression type.</typeparam>
		/// <param name="htmlHelper">HTML helper. Cannot be null.</param>
		/// <param name="expression">Property access expression. Cannot be null.</param>
		/// <param name="binding">Knockout data-bind attribute. Cannot be null.</param>
		/// <param name="cssClass">CSS class attribute. Can be null or empty, in which case this attribute is not specified.</param>
		/// <param name="id">ID attribute. Can be null or empty, in which case this attribute is not specified.</param>
		/// <param name="maxLength">Max length attribute. Can be null, in which case this attribute is not specified.</param>
		/// <param name="size">Size attribute. Can be null, in which case this attribute is not specified.</param>
		/// <returns></returns>
		public static MvcHtmlString TextBoxForKnockout<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, 
			string binding, string cssClass = null, string id = null, int? maxLength = null, int? size = null) {

			ParamIs.NotNull(() => htmlHelper);
			ParamIs.NotNull(() => expression);
			ParamIs.NotNull(() => binding);

			var htmlAttributes = new Dictionary<string, object> { { "data-bind", binding } };

			if (!string.IsNullOrEmpty(cssClass))
				htmlAttributes.Add("class", cssClass);

			if (!string.IsNullOrEmpty(id))
				htmlAttributes.Add("id", id);

			if (maxLength.HasValue)
				htmlAttributes.Add("maxlength", maxLength.Value);

			if (size.HasValue)
				htmlAttributes.Add("size", size.Value);

			return htmlHelper.TextBoxFor(expression, htmlAttributes);

		}

	}

}