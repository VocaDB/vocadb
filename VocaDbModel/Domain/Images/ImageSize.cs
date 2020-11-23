using System;

namespace VocaDb.Model.Domain.Images
{

	public enum ImageSize
	{

		/// <summary>
		/// Original image. 
		/// Typically full size, although a maximum size can be placed as well.
		/// </summary>
		Original = 1,

		/// <summary>
		/// Large thumbnail, default size is 250x250px.
		/// </summary>
		Thumb = 2,

		/// <summary>
		/// Small thumbnail, default size is 150x150px.
		/// </summary>
		SmallThumb = 4,

		/// <summary>
		/// Tiny thumbnail, default size is 70x70px.
		/// </summary>
		TinyThumb = 8,

	}

	[Flags]
	public enum ImageSizes
	{

		Nothing = 0,

		/// <summary>
		/// See <see cref="ImageSize.Original"/>
		/// </summary>
		Original = 1,

		/// <summary>
		/// See <see cref="ImageSize.Thumb"/>
		/// </summary>
		Thumb = 2,

		/// <summary>
		/// See <see cref="ImageSize.SmallThumb"/>
		/// </summary>
		SmallThumb = 4,

		/// <summary>
		/// See <see cref="ImageSize.TinyThumb"/>
		/// </summary>
		TinyThumb = 8,

		AllThumbs = Thumb | SmallThumb | TinyThumb,

		All = Original | Thumb | SmallThumb | TinyThumb

	}

}