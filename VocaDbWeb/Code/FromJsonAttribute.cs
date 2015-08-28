using System;
using System.ComponentModel;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json;
using NLog;

namespace VocaDb.Web.Code {

	/// <summary>
	/// Deserializes a model attribute using the JSON.NET serializer (instead of ASP.NET MVC default serializer).
	/// </summary>
	/// <remarks>
	/// Note that this is only needed if parts of the form are sended as JSON and the rest is standard form.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Property, 
		AllowMultiple = false, Inherited = false)]
	public class FromJsonAttribute : CustomModelBinderAttribute, ICustomPropertyBinder {

		public override IModelBinder GetBinder() {
			return new JsonModelBinder();
		}

		public IPropertyBinder GetPropertyBinder() {
			return new JsonModelBinder();
		}
	}

	public interface ICustomPropertyBinder {

		IPropertyBinder GetPropertyBinder();

	}

	public class JsonModelBinder : IModelBinder, IPropertyBinder {

		private static readonly Attribute allowHtmlAttribute = new AllowHtmlAttribute();
		private static readonly ILogger log = LogManager.GetCurrentClassLogger();

		private object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, string propertyName, Type type, bool allowHtml) {

			var stringified = (!allowHtml ? controllerContext.HttpContext.Request[propertyName] : controllerContext.HttpContext.Request.Unvalidated[propertyName]);

			if (string.IsNullOrEmpty(stringified))
				return null;

			object obj;

			try {
				obj = JsonConvert.DeserializeObject(stringified, type, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			} catch (JsonReaderException x) {
				log.Error(x, "Unable to process JSON, content is " + stringified);
				throw;
			}

			return obj;

		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
			return BindModel(controllerContext, bindingContext, bindingContext.ModelName, bindingContext.ModelType, false);
		}

		public object BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor) {
			return BindModel(controllerContext, bindingContext, propertyDescriptor.Name, propertyDescriptor.PropertyType, propertyDescriptor.Attributes.Contains(allowHtmlAttribute));
		}

	}

}