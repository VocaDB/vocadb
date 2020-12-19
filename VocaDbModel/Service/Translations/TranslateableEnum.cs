#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VocaDb.Model.Service.Translations
{
	public class TranslateableEnum<TEnum> : ITranslateableEnum, IEnumerable<TranslateableEnumField<TEnum>>
		where TEnum : struct, Enum
	{
		private readonly Func<ResourceManager> _resourceManager;

		internal ResourceManager ResourceManager => _resourceManager();

		internal virtual string GetName(string val, ResourceManager res) => res.GetString(val);

		internal virtual string GetName(string val, ResourceManager res, CultureInfo cultureInfo)
		{
			return res.GetString(val, cultureInfo);
		}

		private string GetName(TEnum val, ResourceManager res) => GetName(val.ToString(), res);

		private string GetName(TEnum val, ResourceManager res, CultureInfo cultureInfo)
		{
			return GetName(val.ToString(), res, cultureInfo);
		}

		public TranslateableEnum(Func<ResourceManager> resourceManager)
		{
			this._resourceManager = resourceManager;
			this.Values = EnumVal<TEnum>.Values;
		}

		public TranslateableEnum(Func<ResourceManager> resourceManager, IEnumerable<TEnum> values)
		{
			this._resourceManager = resourceManager;
			this.Values = values.ToArray();
		}

		public string this[TEnum val] => GetName(val);

		public IEnumerable<TranslateableEnumField<TEnum>> AllFields => Values.Select(v => new TranslateableEnumField<TEnum>(v, GetName(v)));

		public Dictionary<string, string> ValuesAndNamesStrings => GetValuesAndNamesStrings(Values);

		public TEnum[] Values { get; }

		public string GetAllNameNames(TEnum flags, params TEnum[] except)
		{
			return string.Join(", ", Values
				.Where(f => !except.Contains(f) && EnumVal<TEnum>.FlagIsSet(flags, f))
				.Select(GetName));
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<TranslateableEnumField<TEnum>> GetEnumerator() => AllFields.GetEnumerator();

		public IEnumerable<TranslateableEnumField<TEnum>> GetTranslatedFields(params TEnum[] values)
		{
			var res = ResourceManager;
			return values.Select(t => new TranslateableEnumField<TEnum>(t, GetName(t, res)));
		}

		public string GetName(TEnum val) => GetName(val, ResourceManager);

		public string GetName(TEnum val, CultureInfo cultureInfo) => GetName(val, ResourceManager, cultureInfo);

		public Dictionary<TEnum, string> GetValuesAndNames(TEnum[] values)
		{
			var res = ResourceManager;
			return values.ToDictionary(t => t, t => GetName(t, res));
		}

		public Dictionary<string, string> GetValuesAndNamesStrings(IEnumerable<TEnum> values)
		{
			var res = ResourceManager;
			return values.ToDictionary(t => t.ToString(), t => GetName(t, res));
		}
	}

	public interface ITranslateableEnum
	{
	}

	public readonly struct TranslateableEnumField<T> where T : struct, IConvertible
	{
		public TranslateableEnumField(T id, string translation)
		{
			Id = id;
			Name = translation;
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public T Id { get; }

		public string Name { get; }

		public KeyValuePair<T, string> ToKeyValuePair() => new KeyValuePair<T, string>(Id, Name);
	}
}