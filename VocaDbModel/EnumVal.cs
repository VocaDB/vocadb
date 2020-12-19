#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model
{
	/// <summary>
	/// Type-safe enum
	/// </summary>
	/// <typeparam name="T">Enum type</typeparam>
	public class EnumVal<T> : IEquatable<EnumVal<T>>, IEquatable<T> where T : struct, Enum
	{
		private T _val;

		private int ValInt
		{
			get => Convert.ToInt32(_val);
			set => _val = (T)Enum.ToObject(typeof(T), value);
		}

		public static T All
		{
			get
			{
				var intVal = Values.Select(v => Convert.ToInt32(v)).Aggregate(0, (current, val) => current | val);
				return (T)Enum.ToObject(typeof(T), intVal);
			}
		}

		/// <summary>
		/// Checks whether a flag has been set.
		/// </summary>
		/// <param name="flags">Flag array.</param>
		/// <param name="flag">Flag to check.</param>
		/// <returns>True if the flag is set.</returns>
		public static bool FlagIsSet(T flags, T flag)
		{
			return (Convert.ToInt32(flags) & Convert.ToInt32(flag)) == Convert.ToInt32(flag);
		}

		/// <summary>
		/// Gets individual values from a bitfield.
		/// </summary>
		/// <param name="flags">Bitfield to be parsed.</param>
		/// <returns>Individual set values.</returns>
		public static T[] GetIndividualValues(T flags)
		{
			return Values.Where(val => FlagIsSet(flags, val)).ToArray();
		}

		public static bool IsDefined(string str) => Enum.IsDefined(typeof(T), str);

		/// <summary>
		/// List of possible values for this enum.
		/// </summary>
		public static T[] Values
		{
			get
			{
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
		public static T Parse(string value)
		{
			return (T)Enum.Parse(typeof(T), value);
		}

		public static T[] ParseAll(string[] values)
		{
			ParamIs.NotNull(() => values);

			var list = new List<T>(values.Length);

			foreach (var name in values)
			{
				if (Enum.TryParse(name, true, out T field))
					list.Add(field);
			}

			return list.ToArray();
		}

		public static T[] ParseMultiple(string value)
		{
			if (string.IsNullOrEmpty(value))
				return new T[0];

			return ParseAll(value.Split(','));
		}

		public static T ParseSafe(string value, T def = default)
			=> Enum.TryParse(value, true, out T val) ? val : def;

		/// <summary>
		/// Initializes a new instance of enum
		/// </summary>
		public EnumVal() { }

		/// <summary>
		/// Initializes a new instance of enum
		/// </summary>
		/// <param name="flags">Enum flags to set.</param>
		public EnumVal(T flags)
		{
			this._val = flags;
		}

		public bool IsDefaultVal => _val.Equals(default(T));

		/// <summary>
		/// Gets or sets the current value
		/// </summary>
		public T Value
		{
			get => _val;
			set => _val = value;
		}

		public void AddFlag(T flag) => ValInt |= Convert.ToInt32(flag);

		public void Clear() => Value = default;

		public override bool Equals(object obj) => Equals(obj as EnumVal<T>);

		public bool Equals(EnumVal<T> other) => other != null && Value.Equals(other.Value);

		public bool Equals(T other) => Value.Equals(other);

		/// <summary>
		/// Checks whether a flag has been set.
		/// </summary>
		/// <param name="flag">Flag to check.</param>
		/// <returns>True if the flag is set.</returns>
		public bool FlagIsSet(T flag)
		{
			return (ValInt & Convert.ToInt32(flag)) == Convert.ToInt32(flag);
		}

		public override int GetHashCode() => Value.GetHashCode();

		public void RemoveFlag(T flag) => ValInt -= Convert.ToInt32(flag);

		public void SetFlag(T flag, bool val)
		{
			var isSet = FlagIsSet(flag);

			if (val && !isSet)
				AddFlag(flag);
			else if (!val && isSet)
				RemoveFlag(flag);
		}
	}
}
