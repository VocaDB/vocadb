#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class ProfileIconKnockoutViewModel
	{
		public ProfileIconKnockoutViewModel(string binding, int size = 80)
		{
			Binding = binding;
			Size = size;
		}

		public string Binding { get; set; }

		public int Size { get; set; }
	}
}