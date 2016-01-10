using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VocaDb.Model.Service.Translations {

	public class TranslateableEnum<TEnum> : ITranslateableEnum where TEnum : struct, IConvertible {

		private readonly Func<ResourceManager> resourceManager;
		private readonly TEnum[] values;

		internal ResourceManager ResourceManager => resourceManager();

		internal virtual string GetName(string val, ResourceManager res) {
			return res.GetString(val);
		}

		internal virtual string GetName(string val, ResourceManager res, CultureInfo cultureInfo) {
			return res.GetString(val, cultureInfo);
		}

		private string GetName(TEnum val, ResourceManager res) {
			return GetName(val.ToString(), res);
		}

		private string GetName(TEnum val, ResourceManager res, CultureInfo cultureInfo) {
			return GetName(val.ToString(), res, cultureInfo);
		}

		public TranslateableEnum(Func<ResourceManager> resourceManager) {
			this.resourceManager = resourceManager;
			this.values = EnumVal<TEnum>.Values;
		}

		public TranslateableEnum(Func<ResourceManager> resourceManager, IEnumerable<TEnum> values) {
			this.resourceManager = resourceManager;
			this.values = values.ToArray();
		}

		public string this[TEnum val] {
			get {
				return GetName(val);
			}
		}

		public IEnumerable<TranslateableEnumField<TEnum>> AllFields {
			get {
				return values.Select(v => new TranslateableEnumField<TEnum>(v, GetName(v)));
			}
		}

		public Dictionary<TEnum, string> ValuesAndNames {
			get {
				return GetValuesAndNames(Values);
			}
		}

		public Dictionary<string, string> ValuesAndNamesStrings {
			get {
				return GetValuesAndNamesStrings(Values);
			}
		}

		public TEnum[] Values {
			get { return values; }
		}

		public string GetAllNameNames(TEnum flags, params TEnum[] except) {

			return string.Join(", ", Values
				.Where(f => !except.Contains(f) && EnumVal<TEnum>.FlagIsSet(flags, f))
				.Select(GetName));

		}

		public IEnumerable<TranslateableEnumField<TEnum>> GetTranslatedFields(params TEnum[] values) {
			var res = ResourceManager;
			return values.Select(t => new TranslateableEnumField<TEnum>(t, GetName(t, res)));
		}

		public string GetName(TEnum val) {
			return GetName(val, ResourceManager);
		}

		public string GetName(TEnum val, CultureInfo cultureInfo) {
			return GetName(val, ResourceManager, cultureInfo);
		}

		public Dictionary<TEnum, string> GetValuesAndNames(TEnum[] values) {
			var res = ResourceManager;
			return values.ToDictionary(t => t, t => GetName(t, res));
		}

		public Dictionary<string, string> GetValuesAndNamesStrings(TEnum[] values) {
			var res = ResourceManager;
			return values.ToDictionary(t => t.ToString(), t => GetName(t, res));
		}

	}

	public interface ITranslateableEnum {
		
	}

	public class TranslateableEnumField<T> where T : struct, IConvertible {

		public TranslateableEnumField(T id, string translation) {
			this.Id = id;
			this.Name = translation;
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public T Id { get; set; }

		public string Name { get; set; }

		public KeyValuePair<T, string> ToKeyValuePair() {
			return new KeyValuePair<T, string>(Id, Name);
		}

	}

}