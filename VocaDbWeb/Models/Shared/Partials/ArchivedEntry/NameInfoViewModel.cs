#nullable disable

using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{
	public class NameInfoViewModel
	{
		public NameInfoViewModel(LocalizedStringContract name)
		{
			Name = name;
		}

		public LocalizedStringContract Name { get; set; }
	}
}