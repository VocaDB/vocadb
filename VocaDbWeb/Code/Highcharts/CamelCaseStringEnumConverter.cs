using Newtonsoft.Json.Converters;

namespace VocaDb.Web.Code.Highcharts
{
	public class CamelCaseStringEnumConverter : StringEnumConverter
	{
		public CamelCaseStringEnumConverter()
		{
			CamelCaseText = true;
		}
	}
}