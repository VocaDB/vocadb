using System;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model {

	/// <summary>
	/// Type-safe enum
	/// </summary>
	/// <typeparam name="T">Enum type</typeparam>
	public class EnumVal<T> where T : struct, IConvertible {

		private T val;

		private int ValInt {
			get { return Convert.ToInt32(val); }
			set {
				val = (T)Enum.ToObject(typeof(T), value);
			}
		}

		/// <summary>
		/// Checks whether a flag has been set.
		/// </summary>
		/// <param name="flags">Flag array.</param>
		/// <param name="flag">Flag to check.</param>
		/// <returns>True if the flag is set.</returns>
		public static bool FlagIsSet(T flags, T flag) {
			return (Convert.ToInt32(flags) & Convert.ToInt32(flag)) == Convert.ToInt32(flag);
		}

		/// <summary>
		/// Gets individual values from a bitfield.
		/// </summary>
		/// <param name="flags">Bitfield to be parsed.</param>
		/// <returns>Individual set values.</returns>
		public static T[] GetIndividualValues(T flags) {
			return Values.Where(val => FlagIsSet(flags, val)).ToArray();
		}

		public static bool IsDefined(string str) {
			return Enum.IsDefined(typeof(T), str);
		}

		/// <summary>
		/// List of possible values for this enum.
		/// </summary>
		public static T[] Values {
			get {
				return (
					from T value in Enum.GetValues(typeof(T))
					select value
					).ToArray();
			}
		}

		/// <summary>
		/// Parses a valid enum value from a string.
		/// </summary>
		/// <param name="value">String representation of the enum value.</param>
		/// <returns>Enum value matching the string.</returns>
		public static T Parse(string value) {
			return (T)Enum.Parse(typeof(T), value);
		}

		public static T[] ParseAll(string[] values) {

			ParamIs.NotNull(() => values);

			var list = new List<T>(values.Length);

			foreach (var name in values) {
				T field;
				if (Enum.TryParse(name, out field))
					list.Add(field);
			}

			return list.ToArray();

		}

		public static T[] ParseMultiple(string value) {

			if (string.IsNullOrEmpty(value))
				return new T[0];

			return ParseAll(value.Split(','));

		}

		public static T ParseSafe(string value, T def) {
			
			T val;
			if (Enum.TryParse(value, true, out val))
				return val;
			else
				return def;

		}

		/// <summary>
		/// Initializes a new instance of enum
		/// </summary>
		public EnumVal() {}

		/// <summary>
		/// Initializes a new instance of enum
		/// </summary>
		/// <param name="flags">Enum flags to set.</param>
		public EnumVal(T flags) {
			this.val = flags;
		}

		/// <summary>
		/// Gets or sets the current value
		/// </summary>
		public T Value {
			get { return val; }
			set { val = value; }
		}

		public void AddFlag(T flag) {
			ValInt |= Convert.ToInt32(flag);	
		}

		public void Clear() {
			Value = default(T);
		}

		/// <summary>
		/// Checks whether a flag has been set.
		/// </summary>
		/// <param name="flag">Flag to check.</param>
		/// <returns>True if the flag is set.</returns>
		public bool FlagIsSet(T flag) {
			return (ValInt & Convert.ToInt32(flag)) == Convert.ToInt32(flag);
		}

		public void RemoveFlag(T flag) {
			ValInt -= Convert.ToInt32(flag);
		}

		public void SetFlag(T flag, bool val) {

			var isSet = FlagIsSet(flag);

			if (val && !isSet)
				AddFlag(flag);
			else if (!val && isSet)
				RemoveFlag(flag);

		}

	}

}
