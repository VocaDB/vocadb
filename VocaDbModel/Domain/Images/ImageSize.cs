#nullable disable

using System;

namespace VocaDb.Model.Domain.Images
{
	public enum ImageSize
	{
		/// <summary>
		/// Original image. 
		/// Typically full size, although a maximum size can be placed as well.
		/// </summary>
		Original = 1 << 0,

		/// <summary>
		/// Large thumbnail, default size is 250x250px.
		/// </summary>
		Thumb = 1 << 1,

		/// <summary>
		/// Small thumbnail, default size is 150x150px.
		/// </summary>
		SmallThumb = 1 << 2,

		/// <summary>
		/// Tiny thumbnail, default size is 70x70px.
		/// </summary>
		TinyThumb = 1 << 3,
	}

	[Flags]
	public enum ImageSizes
	{
		Nothing = 0,

		/// <summary>
		/// See <see cref="ImageSize.Original"/>
		/// </summary>
		Original = 1 << 0,

		/// <summary>
		/// See <see cref="ImageSize.Thumb"/>
		/// </summary>
		Thumb = 1 << 1,

		/// <summary>
		/// See <see cref="ImageSize.SmallThumb"/>
		/// </summary>
		SmallThumb = 1 << 2,

		/// <summary>
		/// See <see cref="ImageSize.TinyThumb"/>
		/// </summary>
		TinyThumb = 1 << 3,

		AllThumbs = Thumb | SmallThumb | TinyThumb,

		All = Original | Thumb | SmallThumb | TinyThumb,
	}
}