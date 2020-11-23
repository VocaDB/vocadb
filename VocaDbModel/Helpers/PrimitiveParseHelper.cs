namespace VocaDb.Model.Helpers
{

	public static class PrimitiveParseHelper
	{

		public static int ParseIntOrDefault(string str, int def)
		{

			int val;
			if (int.TryParse(str, out val))
				return val;
			else
				return def;

		}

	}

}
