using System.ComponentModel;
using System.Web.Mvc;

namespace VocaDb.Web.Code
{
	/// <summary>
	/// Custom property binder.
	/// </summary>
	public interface IPropertyBinder
	{
		object BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor);
	}
}
