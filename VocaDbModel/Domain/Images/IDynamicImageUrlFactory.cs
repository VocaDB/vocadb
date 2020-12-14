#nullable disable

namespace VocaDb.Model.Domain.Images
{
	/// <summary>
	/// Returns "dynamic" MVC controller URL to database-stored images,
	/// that is artist and album original images.
	/// </summary>
	public interface IDynamicImageUrlFactory : IEntryImageUrlFactory { }
}
