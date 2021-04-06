#nullable disable

using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Models.Shared.Partials.Tag
{
	public class TagUsagesManageTableViewModel
	{
		public TagUsagesManageTableViewModel(EntryType entryType, ServerOnlyTagUsageWithVotesContract[] tagUsages, bool canRemove, string controllerName = null)
		{
			EntryType = entryType;
			TagUsages = tagUsages;
			CanRemove = canRemove;
			ControllerName = controllerName;
		}

		public EntryType EntryType { get; set; }

		public ServerOnlyTagUsageWithVotesContract[] TagUsages { get; set; }

		public bool CanRemove { get; set; }

		public string ControllerName { get; set; }
	}
}