#nullable disable

using System;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using VocaDb.Model;

namespace VocaDb.Web.Code
{
	/// <summary>
	/// Parses enum arrays that are passed as comma-separated string (such as "English,Japanese"), similar to bitfields.
	/// </summary>
	/// <remarks>
	/// This version uses reflection to call the correct parsing method based on parameter type. 
	/// Unlike bitfields, the default value is also supported because the result is an array.
	/// More straightforward to use, but possibly slower.
	/// </remarks>
	public class EnumArrayModelBinder : IModelBinder
	{
		private static readonly Type s_genericEnumValType = typeof(EnumVal<>);

		public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
		{
			var key = bindingContext.ModelName;
			var val = bindingContext.ValueProvider.GetValue(key);

			// Value is empty, skip evaluation
			if (string.IsNullOrEmpty(val?.AttemptedValue))
				return false;

			// Get parameter type (should be an array)
			var paramType = bindingContext.ModelType;

			// EnumType[] -> EnumType
			if (paramType.IsArray)
			{
				paramType = paramType.GetElementType();
			}

			// EnumVal<> -> EnumVal<EnumType>
			var enumValType = s_genericEnumValType.MakeGenericType(paramType);

			// EnumVal<EnumType>.ParseMultiple()
			var result = enumValType.GetMethod("ParseMultiple", BindingFlags.Public | BindingFlags.Static).Invoke(null, new[] { val.AttemptedValue });
			bindingContext.Model = result;
			return true;
		}
	}

	public class EnumArrayModelBinder<TEnum> : IModelBinder where TEnum : struct, Enum
	{
		public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
		{
			var key = bindingContext.ModelName;
			var val = bindingContext.ValueProvider.GetValue(key);
			if (val != null && !string.IsNullOrEmpty(val.AttemptedValue))
			{
				var result = EnumVal<TEnum>.ParseMultiple(val.AttemptedValue);
				bindingContext.Model = result;
				return true;
			}

			return false;
		}
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	public class EnumArrayBinderAttribute : ModelBinderAttribute
	{
		public EnumArrayBinderAttribute() : base(typeof(EnumArrayModelBinder)) { }
	}
}