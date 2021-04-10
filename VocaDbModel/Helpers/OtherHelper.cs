using System;

namespace VocaDb.Model.Helpers
{
	public static class OtherHelper
	{
		public static bool HasCircularReference<T>(T? root, Func<T, T?> accessor) where T : class
		{
			return HasObject(root, root, accessor);
		}

		public static bool HasObject<T>(T? root, T? needle, Func<T, T?> accessor) where T : class
		{
			var current = root;
			var iterations = 0;
			while (current != null && ++iterations < 1000)
			{
				current = accessor(current);

				if (Equals(current, needle))
				{
					return true;
				}
			}

			return false;
		}
	}
}
