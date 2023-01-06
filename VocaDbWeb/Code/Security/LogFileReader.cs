using NLog;
using NLog.Targets;

namespace VocaDb.Web.Code.Security;

public class LogFileReader
{
	public async Task<string> GetLatestLogFileContents()
	{
		var fileTarget = (FileTarget?)LogManager.Configuration.ConfiguredNamedTargets.FirstOrDefault(t => t is FileTarget);

		if (fileTarget == null)
			return string.Empty;

		var logFile = fileTarget.FileName.Render(new LogEventInfo());
		var folder = Path.GetDirectoryName(logFile);
		var latestFile = (folder != null ? Directory.GetFiles(folder, "*.log").OrderByDescending(f => f).FirstOrDefault() : null);

		if (latestFile == null || !File.Exists(latestFile))
			return string.Empty;

		// https://github.com/VocaDB/vocadb/issues/1289#issuecomment-1371174365
		using var fileStream = new FileStream(latestFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		using var streamReader = new StreamReader(fileStream);
		return await streamReader.ReadToEndAsync();
	}
}
