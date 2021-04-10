namespace VocaDb.Model.Helpers
{
	public static class PrimitiveParseHelper
	{
		public static int ParseIntOrDefault(string? str, int def) => int.TryParse(str, out int val) ? val : def;
	}
}
