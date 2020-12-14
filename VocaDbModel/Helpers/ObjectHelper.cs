#nullable disable

using System;

namespace VocaDb.Model.Helpers
{
	public static class ObjectHelper
	{
		public static TResult Convert<TSource, TResult>(TSource source, Func<TSource, TResult> func) where TSource : class where TResult : class
		{
			return source != null ? func(source) : null;
		}
	}
}
