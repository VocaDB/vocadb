using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace VocaDb.Web.Code {

	public class CultureInvariantDateTimeModelBinder : IModelBinder {

		public const string Format = "yyyy.MM.dd";

		private readonly string name;

		public CultureInvariantDateTimeModelBinder(string name) {
			this.name = name;
		}

		private string GetValue(ModelBindingContext bindingContext, string key) {
			
			var result = bindingContext.ValueProvider.GetValue(key);
			return (result == null) ? null : result.AttemptedValue;
		
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

			var val = GetValue(bindingContext, name);

			DateTime parsed;

			if (DateTime.TryParseExact(val, Format, null, DateTimeStyles.None, out parsed))
				return parsed;

			return null;

		}

	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum |AttributeTargets.Interface 
		| AttributeTargets.Parameter |AttributeTargets.Struct 
		| AttributeTargets.Property,AllowMultiple = false, Inherited = false)]
	public class CultureInvariantDateTimeModelBinderAttribute : CustomModelBinderAttribute {

		private readonly string name;

		public CultureInvariantDateTimeModelBinderAttribute(string name) {
			this.name = name;
		}

		public override IModelBinder GetBinder()  {
			return new CultureInvariantDateTimeModelBinder(name);  
		}
	}

}