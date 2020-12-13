using VocaDb.Model.Domain.Images;

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class ProfileIconKnockout_ImageSizeViewModel
	{
		public ProfileIconKnockout_ImageSizeViewModel(ImageSize imageSize = ImageSize.TinyThumb, string binding = "$data", int size = 0)
		{
			ImageSize = imageSize;
			Binding = binding;
			Size = size;
		}

		public ImageSize ImageSize { get; set; }

		public string Binding { get; set; }

		public int Size { get; set; }
	}
}