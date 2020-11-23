using System.IO;
using System.Linq;
using NLog;
using NLog.Targets;

namespace VocaDb.Web.Code.Security
{
	public class LogFileReader
	{
		public string GetLatestLogFileContents()
		{
			var fileTarget = (FileTarget)LogManager.Configuration.ConfiguredNamedTargets.FirstOrDefault(t => t is FileTarget);

			if (fileTarget == null)
				return string.Empty;

			var logFile = fileTarget.FileName.Render(new LogEventInfo());
			var folder = Path.GetDirectoryName(logFile);
			var latestFile = (folder != null ? Directory.GetFiles(folder, "*.log").OrderByDescending(f => f).FirstOrDefault() : null);

			if (latestFile == null || !File.Exists(latestFile))
				return string.Empty;

			return File.ReadAllText(latestFile);
		}
	}
}