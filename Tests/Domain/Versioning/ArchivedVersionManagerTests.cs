#nullable disable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Tests.TestData;

namespace VocaDb.Tests.Domain.Versioning
{
	/// <summary>
	/// Tests for <see cref="ArchivedVersionManager{TVersion,TField}"/>.
	/// </summary>
	[TestClass]
	public class ArchivedVersionManagerTests
	{
		private ArchivedVersionManager<ArchivedSongVersion, SongEditableFields> _archivedVersionManager;
		private Song _entry;

		private ArchivedSongVersion CreateVersion()
		{
			var version = _entry.CreateArchivedVersion(new System.Xml.Linq.XDocument(), new SongDiff(), new AgentLoginData("Miku"), SongArchiveReason.Unknown, string.Empty);
			_archivedVersionManager.Add(version);
			return version;
		}

		private ArchivedSongVersion CreateVersion(int versionNumber)
		{
			var version = new ArchivedSongVersion(_entry, new System.Xml.Linq.XDocument(), new SongDiff(), new AgentLoginData("Miku"), versionNumber, _entry.Status, SongArchiveReason.Unknown, string.Empty);
			_archivedVersionManager.Add(version);
			return version;
		}

		[TestInitialize]
		public void SetUp()
		{
			_archivedVersionManager = new ArchivedVersionManager<ArchivedSongVersion, SongEditableFields>();
			_entry = CreateEntry.Song();
		}

		[TestMethod]
		public void GetLatestVersion()
		{
			CreateVersion();
			CreateVersion();
			var version3 = CreateVersion();
			Assert.AreEqual(3, _archivedVersionManager.Versions.Count, "Number of versions");

			var latest = _archivedVersionManager.GetLatestVersion();
			Assert.AreSame(version3, latest, "Latest version is the 3rd version");
			Assert.AreEqual(2, latest.Version, "Latest version number");
		}
	}
}
