#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace VocaDb.Model
{
	/// <summary>
	/// Utility methods for parameter validation.
	/// </summary>
	[DebuggerStepThrough]
	public static class ParamIs
	{
		/// <summary>
		/// Validates that the given parameter is a positive (greater than 0) integer.
		/// </summary>
		/// <param name="expression">Parameter expression. Expression body must be a member expression, for example "() => param". Cannot be null.</param>
		public static void Between(Expression<Func<int>> expression, int min, int max)
		{
			if (!(expression.Body is MemberExpression))
				throw new ArgumentException("Only member expressions are supported", "expression");

			var f = expression.Compile();

			if (f() < min || f() > max)
			{
				var body = (MemberExpression)expression.Body;
				throw new ArgumentException(body.Member.Name + " must be between " + min + " and " + max, body.Member.Name);
			}
		}

		/// <summary>
		/// Validates that the given parameter is a positive (greater than 0) integer.
		/// </summary>
		/// <param name="val">Parameter to be validated.</param>
		/// <param name="paramName">Parameter name. Cannot be null or empty.</param>
		public static void NonNegative(int val, string paramName = null)
		{
			if (val < 0)
				throw new ArgumentException(paramName + " must be a non-negative integer", paramName);
		}

		/// <summary>
		/// Validates that the given parameter is a positive (greater than 0) integer.
		/// </summary>
		/// <param name="expression">Parameter expression. Expression body must be a member expression, for example "() => param". Cannot be null.</param>
		public static void NonNegative(Expression<Func<int>> expression)
		{
			if (!(expression.Body is MemberExpression))
				throw new ArgumentException("Only member expressions are supported", "expression");

			var f = expression.Compile();

			if (f() < 0)
			{
				var body = (MemberExpression)expression.Body;
				throw new ArgumentException(body.Member.Name + " must be a non-negative integer", body.Member.Name);
			}
		}

		/// <summary>
		/// Validates that the given parameter is not null.
		/// </summary>
		/// <param name="val">Parameter to be validated.</param>
		/// <param name="paramName">Parameter name. Cannot be null or empty.</param>
		/// <remarks>
		/// The overload <see cref="NotNull{Expression}"/> should be preferred if possible.
		/// </remarks>
		public static void NotNull(object val, string paramName = null)
		{
			if (val == null)
				throw new ArgumentNullException(paramName);
		}

		/*
		/// <summary>
		/// Validates that the given parameter is not null.
		/// </summary>
		/// <param name="expression">Parameter expression. Expression body must be a member expression, for example "() => param". Cannot be null.</param>
		public static void NotNull<T>(Expression<Func<T>> expression) where T : class {

			if (!(expression.Body is MemberExpression))
				throw new ArgumentException("Only member expressions are supported", "expression");

			var f = expression.Compile();

			if (f() == null) {
				var body = (MemberExpression)expression.Body;
				throw new ArgumentNullException(body.Member.Name);				
			}
		}*/

		/// <summary>
		/// Validates that the given parameter is not null.
		/// </summary>
		/// <param name="arg">Parameter expression. Expression body must be a member expression, for example "() => param". Cannot be null.</param>
		public static void NotNull<T>(Func<T> arg) where T : class
		{
			if (arg() != null)
				return;

			var test = new FieldInfoReader<T>(arg);

			FieldInfo fieldInfo = test.GetFieldToken();

			if (fieldInfo == null)
			{
				throw new ValidationException("No field info found in delegate");
			}

			throw new ArgumentNullException(fieldInfo.Name);
		}

		/// <summary>
		/// Validates that the given string parameter is not null or empty.
		/// </summary>
		/// <param name="val">Parameter to be validated.</param>
		/// <param name="paramName">Parameter name. Cannot be null or empty.</param>
		/// <remarks>
		/// The overload <see cref="NotNullOrEmpty(Expression{Func{string}})"/> should be preferred if possible.
		/// </remarks>
		public static void NotNullOrEmpty(string val, string paramName = null)
		{
			if (string.IsNullOrEmpty(val))
				throw new ArgumentException(paramName + " cannot be null or empty", paramName);
		}

		/*
		/// <summary>
		/// Validates that the given parameter is not null or empty.
		/// </summary>
		/// <param name="expression">Parameter expression. Expression body must be a member expression. Cannot be null.</param>
		public static void NotNullOrEmpty(Expression<Func<string>> expression) {

			if (!(expression.Body is MemberExpression))
				throw new ArgumentException("Only member expressions are supported", "expression");

			var f = expression.Compile();

			if (string.IsNullOrEmpty(f())) {
				var body = (MemberExpression)expression.Body;
				throw new ArgumentException(body.Member.Name + " cannot be null or empty", body.Member.Name);
			}
		}*/

		/// <summary>
		/// Validates that the given parameter is not null or empty.
		/// </summary>
		/// <param name="arg">Parameter expression. Expression body must be a member expression. Cannot be null.</param>
		public static void NotNullOrEmpty(Func<string> arg)
		{
			if (!string.IsNullOrEmpty(arg()))
				return;

			var test = new FieldInfoReader<string>(arg);

			FieldInfo fieldInfo = test.GetFieldToken();

			if (fieldInfo == null)
			{
				throw new ValidationException("No field info found in delegate");
			}

			throw new ArgumentException(fieldInfo.Name + " cannot be null or empty");
		}

		/// <summary>
		/// Validates that the given parameter is not null or whitespace.
		/// </summary>
		/// <param name="expression">Parameter expression. Expression body must be a member expression. Cannot be null.</param>
		public static void NotNullOrWhiteSpace(Expression<Func<string>> expression)
		{
			if (!(expression.Body is MemberExpression))
				throw new ArgumentException("Only member expressions are supported", "expression");

			var f = expression.Compile();

			if (string.IsNullOrWhiteSpace(f()))
			{
				var body = (MemberExpression)expression.Body;
				throw new ArgumentException(body.Member.Name + " cannot be null or whitespace", body.Member.Name);
			}
		}

		/// <summary>
		/// Validates that the given parameter is a positive (greater than 0) integer.
		/// </summary>
		/// <param name="val">Parameter to be validated.</param>
		/// <param name="paramName">Parameter name. Cannot be null or empty.</param>
		public static void Positive(int val, string paramName = null)
		{
			if (val <= 0)
				throw new ArgumentException(paramName + " must be a positive integer", paramName);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TParameter"></typeparam>
	/// <remarks>Code from http://bronumski.blogspot.com/2010/06/taking-pain-out-of-parameter-validation.html </remarks>
	internal class FieldInfoReader<TParameter>
	{
		private readonly Func<TParameter> arg;

		internal FieldInfoReader(Func<TParameter> arg)
		{
			this.arg = arg;
		}

		public FieldInfo GetFieldToken()
		{
			byte[] methodBodyIlByteArray = GetMethodBodyIlByteArray();

			int fieldToken = GetFieldToken(methodBodyIlByteArray);

			return GetFieldInfo(fieldToken);
		}

		private FieldInfo GetFieldInfo(int fieldToken)
		{
			FieldInfo fieldInfo = null;

			if (fieldToken > 0)
			{
				Type argType = arg.Target.GetType();
				Type[] genericTypeArguments = GetSubclassGenericTypes(argType);
				Type[] genericMethodArguments = arg.Method.GetGenericArguments();

				fieldInfo = argType.Module.ResolveField(fieldToken, genericTypeArguments, genericMethodArguments);
			}

			return fieldInfo;
		}

		private static OpCode GetOpCode(byte[] methodBodyIlByteArray, ref int currentPosition)
		{
			ushort value = methodBodyIlByteArray[currentPosition++];

			return value != 0xfe ? SingleByteOpCodes[value] : OpCodes.Nop;
		}

		private static int GetFieldToken(byte[] methodBodyIlByteArray)
		{
			int position = 0;

			while (position < methodBodyIlByteArray.Length)
			{
				OpCode code = GetOpCode(methodBodyIlByteArray, ref position);

				if (code.OperandType == OperandType.InlineField)
				{
					return ReadInt32(methodBodyIlByteArray, ref position);
				}

				position = MoveToNextPosition(position, code);
			}

			return 0;
		}

		private static int MoveToNextPosition(int position, OpCode code)
		{
			switch (code.OperandType)
			{
				case OperandType.InlineNone:
					break;

				case OperandType.InlineI8:
				case OperandType.InlineR:
					position += 8;
					break;

				case OperandType.InlineField:
				case OperandType.InlineBrTarget:
				case OperandType.InlineMethod:
				case OperandType.InlineSig:
				case OperandType.InlineTok:
				case OperandType.InlineType:
				case OperandType.InlineI:
				case OperandType.InlineString:
				case OperandType.InlineSwitch:
				case OperandType.ShortInlineR:
					position += 4;
					break;

				case OperandType.InlineVar:
					position += 2;
					break;

				case OperandType.ShortInlineBrTarget:
				case OperandType.ShortInlineI:
				case OperandType.ShortInlineVar:
					position++;
					break;

				default:
					throw new InvalidOperationException("Unknown operand type.");
			}
			return position;
		}

		private byte[] GetMethodBodyIlByteArray()
		{
			MethodBody methodBody = arg.Method.GetMethodBody();

			if (methodBody == null)
			{
				throw new InvalidOperationException();
			}

			return methodBody.GetILAsByteArray();
		}

		private static int ReadInt32(byte[] il, ref int position)
		{
			return ((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18);
		}

		private static Type[] GetSubclassGenericTypes(Type toCheck)
		{
			var genericArgumentsTypes = new List<Type>();

			while (toCheck != null)
			{
				if (toCheck.IsGenericType)
				{
					genericArgumentsTypes.AddRange(toCheck.GetGenericArguments());
				}

				toCheck = toCheck.BaseType;
			}

			return genericArgumentsTypes.ToArray();
		}

		private static OpCode[] singleByteOpCodes;

		public static OpCode[] SingleByteOpCodes
		{
			get
			{
				if (singleByteOpCodes == null)
				{
					LoadOpCodes();
				}
				return singleByteOpCodes;
			}
		}

		private static void LoadOpCodes()
		{
			singleByteOpCodes = new OpCode[0x100];

			FieldInfo[] opcodeFieldInfos = typeof(OpCodes).GetFields();

			for (int i = 0; i < opcodeFieldInfos.Length; i++)
			{
				FieldInfo info1 = opcodeFieldInfos[i];

				if (info1.FieldType == typeof(OpCode))
				{
					var singleByteOpCode = (OpCode)info1.GetValue(null);

					var singleByteOpcodeIndex = (ushort)singleByteOpCode.Value;

					if (singleByteOpcodeIndex < 0x100)
					{
						singleByteOpCodes[singleByteOpcodeIndex] = singleByteOpCode;
					}
				}
			}
		}
	}
}
