#nullable disable


namespace VocaDb.Web.Models.Admin
{
	public class ViewSysLog
	{
		public ViewSysLog(string logContents)
		{
			LogContents = logContents;
		}

		public string LogContents { get; set; }
	}
}