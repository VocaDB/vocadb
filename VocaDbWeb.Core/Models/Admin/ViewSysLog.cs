#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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