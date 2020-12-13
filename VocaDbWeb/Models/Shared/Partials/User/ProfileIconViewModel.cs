using VocaDb.Model.Helpers;

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class ProfileIconViewModel
	{
		public ProfileIconViewModel(string url, int size = ImageHelper.UserThumbSize)
		{
			Url = url;
			Size = size;
		}

		public string Url { get; set; }

		public int Size { get; set; }
	}
}