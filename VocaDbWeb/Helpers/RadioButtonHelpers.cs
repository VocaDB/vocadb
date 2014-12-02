using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using VocaDb.Web.Helpers.Support;

namespace VocaDb.Web.Helpers {
	public static class RadioButtonHelpers {

		public class RadioButtonListViewModel<T> {
			public string Id { get; set; }
			private T selectedValue;
			public T SelectedValue {
				get { return selectedValue; }
				set {
					selectedValue = value;
					UpdatedSelectedItems();
				}
			}

			private void UpdatedSelectedItems() {
				if (ListItems == null)
					return;

				ListItems.ForEach(li => li.Selected = Equals(li.Value, SelectedValue));
			}

			private List<RadioButtonListItem<T>> listItems;
			public List<RadioButtonListItem<T>> ListItems {
				get { return listItems; }
				set {
					listItems = value;
					UpdatedSelectedItems();
				}
			}
		}

		public class RadioButtonListItem<T> {
			public bool Selected { get; set; }

			public string Text { get; set; }

			public T Value { get; set; }

			public override string ToString() {
				return Value.ToString();
			}
		}

		public static string RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class {
			return htmlHelper.RadioButtonListFor(expression, null);
		}

		public static string RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes) where TModel : class {
			return htmlHelper.RadioButtonListFor(expression, new RouteValueDictionary(htmlAttributes));
		}

		public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes) {
			var inputName = GetInputName(expression);

			//RadioButtonListViewModel<TRadioButtonListValue> radioButtonList = GetValue(htmlHelper, expression);

			/*if (radioButtonList == null)
				return String.Empty;

			if (radioButtonList.ListItems == null)
				return String.Empty;*/

			var divTag = new TagBuilder("span");
			divTag.MergeAttribute("id", inputName);
			divTag.MergeAttribute("class", "radio");
			foreach (var item in selectList) {
				var radioButtonTag = RadioButton(htmlHelper, inputName, item, htmlAttributes);

				divTag.InnerHtml += radioButtonTag;
			}

			return new MvcHtmlString(divTag + htmlHelper.ValidationMessage(inputName, "*").ToHtmlString());
		}

		// Not in use currently
		public static MvcHtmlString EnumRadioButtonListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TEnum>> expression,
			TranslateableEnum<TEnum> enumType, IDictionary<string, object> htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, IConvertible {

			return RadioButtonListFor(htmlHelper, expression, ViewHelper.CreateEnumList(selectedValue, enumType), htmlAttributes);

		}

		// Not in use currently
		public static MvcHtmlString EnumRadioButtonListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TEnum>> expression,
			IEnumerable<KeyValuePair<TEnum, string>> values, IDictionary<string, object> htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, IConvertible {

			return RadioButtonListFor(htmlHelper, expression, ViewHelper.CreateEnumList(selectedValue, values), htmlAttributes);

		}

		public static string GetInputName<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression) {
			if (expression.Body.NodeType == ExpressionType.Call) {
				var methodCallExpression = (MethodCallExpression)expression.Body;
				string name = GetInputName(methodCallExpression);
				return name.Substring(expression.Parameters[0].Name.Length + 1);

			}
			return expression.Body.ToString().Substring(expression.Parameters[0].Name.Length + 1);
		}

		private static string GetInputName(MethodCallExpression expression) {
			// p => p.Foo.Bar().Baz.ToString() => p.Foo OR throw...

			var methodCallExpression = expression.Object as MethodCallExpression;
			if (methodCallExpression != null) {
				return GetInputName(methodCallExpression);
			}
			return expression.Object.ToString();
		}

		public static string RadioButton(this HtmlHelper htmlHelper, string name, SelectListItem listItem,
							 IDictionary<string, object> htmlAttributes) {
			var inputIdSb = new StringBuilder();
			inputIdSb.Append(name)
				.Append("_")
				.Append(listItem.Value);

			var sb = new StringBuilder();

			var labelBuilder = new TagBuilder("label");
			//labelBuilder.MergeAttribute("for", inputId);
			labelBuilder.MergeAttributes(htmlAttributes);
			labelBuilder.InnerHtml = listItem.Text;

			var builder = new TagBuilder("input");
			if (listItem.Selected) builder.MergeAttribute("checked", "checked");
			builder.MergeAttribute("type", "radio");
			builder.MergeAttribute("value", listItem.Value);
			builder.MergeAttribute("id", inputIdSb.ToString());
			builder.MergeAttribute("name", name);
			builder.MergeAttributes(htmlAttributes);
			labelBuilder.InnerHtml = builder.ToString(TagRenderMode.SelfClosing) + listItem.Text;
			sb.Append(labelBuilder.ToString());
			//sb.Append(builder.ToString(TagRenderMode.SelfClosing));
			//sb.Append(RadioButtonLabel(inputIdSb.ToString(), listItem.Text, htmlAttributes));
			//sb.Append("<br>");

			return sb.ToString();
		}

		public static string RadioButtonLabel(string inputId, string displayText,
									 IDictionary<string, object> htmlAttributes) {
			var labelBuilder = new TagBuilder("label");
			labelBuilder.MergeAttribute("for", inputId);
			labelBuilder.MergeAttributes(htmlAttributes);
			labelBuilder.InnerHtml = displayText;

			return labelBuilder.ToString(TagRenderMode.Normal);
		}


		public static TProperty GetValue<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class {
			TModel model = htmlHelper.ViewData.Model;
			if (model == null) {
				return default(TProperty);
			}
			Func<TModel, TProperty> func = expression.Compile();
			return func(model);
		}
	}
}