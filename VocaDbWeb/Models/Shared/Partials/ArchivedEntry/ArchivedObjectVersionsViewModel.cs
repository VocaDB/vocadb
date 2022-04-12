#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{
	using Antlr.Runtime.Misc;

	public class ArchivedObjectVersionsViewModel
	{
		public ArchivedObjectVersionsViewModel(IEnumerable<ArchivedObjectVersion> archivedVersions, Func<int, string> linkFunc = null)
		{
			ArchivedVersions = archivedVersions;
			LinkFunc = linkFunc;
		}

		public IEnumerable<ArchivedObjectVersion> ArchivedVersions { get; set; }

		public Func<int, string> LinkFunc { get; set; }
	}
}