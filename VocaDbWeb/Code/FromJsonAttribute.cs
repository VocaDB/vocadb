using System;
using System.ComponentModel;
using System.Web.Mvc;
using Newtonsoft.Json;

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

		private object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, string propertyName, Type type) {
			var stringified = controllerContext.HttpContext.Request[propertyName];
			if (string.IsNullOrEmpty(stringified))
				return null;
			var obj = JsonConvert.DeserializeObject(stringified, type, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			return obj;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
			return BindModel(controllerContext, bindingContext, bindingContext.ModelName, bindingContext.ModelType);
		}

		public object BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor) {
			return BindModel(controllerContext, bindingContext, propertyDescriptor.Name, propertyDescriptor.PropertyType);
		}

	}

}