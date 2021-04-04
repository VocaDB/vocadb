#nullable disable

using System;
using System.Collections.Generic;

namespace VocaDb.Model.Helpers
{
	public class LambdaComparer<T> : IComparer<T>
	{
		private readonly Func<T, T, int> _comparer;

#nullable enable
		public LambdaComparer(Func<T, T, int> comparer)
		{
			ParamIs.NotNull(() => comparer);
			_comparer = comparer;
		}
#nullable disable

		public int Compare(T x, T y)
		{
			return _comparer(x, y);
		}
	}
}
