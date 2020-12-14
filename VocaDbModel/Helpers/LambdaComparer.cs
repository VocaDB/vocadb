#nullable disable

using System;
using System.Collections.Generic;

namespace VocaDb.Model.Helpers
{
	public class LambdaComparer<T> : IComparer<T>
	{
		private readonly Func<T, T, int> comparer;

		public LambdaComparer(Func<T, T, int> comparer)
		{
			ParamIs.NotNull(() => comparer);
			this.comparer = comparer;
		}

		public int Compare(T x, T y)
		{
			return comparer(x, y);
		}
	}
}
