namespace VocaDb.Web.Code.Highcharts
{

	public class Title
	{

		public static implicit operator Title(string text)
		{
			return new Title(text);
		}

		public Title(string text = null)
		{
			Text = text;
		}

		public string Text { get; set; }

	}

}