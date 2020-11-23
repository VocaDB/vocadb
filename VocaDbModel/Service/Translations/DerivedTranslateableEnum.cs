using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.Translations
{
	// Supports loading translations from a "base" enum
	public class DerivedTranslateableEnum<TEnum, TEnumBase> : TranslateableEnum<TEnum> where TEnum : struct, Enum where TEnumBase : struct, Enum
	{
		private readonly TranslateableEnum<TEnumBase> baseEnum;

		internal override string GetName(string val, ResourceManager res, CultureInfo cultureInfo)
		{
			return base.GetName(val, res, cultureInfo).EmptyToNull() ?? baseEnum.GetName(val, baseEnum.ResourceManager, cultureInfo);
		}

		internal override string GetName(string val, ResourceManager res)
		{
			return base.GetName(val, res).EmptyToNull() ?? baseEnum.GetName(val, baseEnum.ResourceManager);
		}

		public DerivedTranslateableEnum(TranslateableEnum<TEnumBase> baseEnum, Func<ResourceManager> resourceManager) : base(resourceManager)
		{
			this.baseEnum = baseEnum;
		}

		public DerivedTranslateableEnum(TranslateableEnum<TEnumBase> baseEnum, Func<ResourceManager> resourceManager, IEnumerable<TEnum> values) : base(resourceManager, values)
		{
			this.baseEnum = baseEnum;
		}
	}
}