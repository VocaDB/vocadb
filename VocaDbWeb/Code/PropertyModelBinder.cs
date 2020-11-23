using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace VocaDb.Web.Code
{
	/// <summary>
	/// Allows custom property bindings.
	/// If a property is decorated with a subclass of <see cref="CustomModelBinderAttribute"/>, that binder will be used for that property.
	/// </summary>
	public class PropertyModelBinder : DefaultModelBinder
	{
		protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			// Check if the property has the PropertyBinderAttribute, meaning
			// it's specifying a different binder to use.
			var propertyBinderAttribute = TryFindPropertyBinderAttribute(propertyDescriptor);
			if (propertyBinderAttribute != null)
			{
				var binder = propertyBinderAttribute.GetPropertyBinder();
				var value = binder.BindProperty(controllerContext, bindingContext, propertyDescriptor);
				propertyDescriptor.SetValue(bindingContext.Model, value);
			}
			else // revert to the default behavior.
			{
				base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
			}
		}

		private ICustomPropertyBinder TryFindPropertyBinderAttribute(PropertyDescriptor propertyDescriptor)
		{
			return propertyDescriptor.Attributes
			  .OfType<ICustomPropertyBinder>()
			  .FirstOrDefault();
		}
	}

	/// <summary>
	/// Parameter or class with this attribute will use the <see cref="PropertyModelBinder"/> binder, 
	/// which allows custom property bindings.
	/// </summary>
	public class PropertyModelBinderAttribute : CustomModelBinderAttribute
	{
		public override IModelBinder GetBinder()
		{
			return new PropertyModelBinder();
		}
	}
}