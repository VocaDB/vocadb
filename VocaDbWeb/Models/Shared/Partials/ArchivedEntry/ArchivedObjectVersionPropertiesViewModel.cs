namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{

	public class ArchivedObjectVersionPropertiesViewModel
	{

		public ArchivedObjectVersionPropertiesViewModel(ArchivedObjectVersion ver, ArchivedObjectVersion compareTo = null)
		{
			Ver = ver;
			CompareTo = compareTo;
		}

		public ArchivedObjectVersion Ver { get; set; }

		public ArchivedObjectVersion CompareTo { get; set; }

	}

}