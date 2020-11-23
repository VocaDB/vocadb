namespace VocaDb.Web.Models.Shared.Partials.Knockout
{
	public class IconAndNameKnockoutViewModel
	{
		public IconAndNameKnockoutViewModel(string iconBinding, string nameBinding, int size = 20)
		{
			IconBinding = iconBinding;
			NameBinding = nameBinding;
			Size = size;
		}

		public string IconBinding { get; set; }

		public string NameBinding { get; set; }

		public int Size { get; set; }
	}
}