#nullable disable

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Web.Code;

namespace VocaDb.Tests.TestSupport
{
	public class FakeDynamicImageUrlFactory : IDynamicImageUrlFactory
	{
		private Lazy<IUrlHelper> UrlHelper => new(() => new UrlHelperFactory().GetUrlHelper(context: null/* FIXME */));

		public VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size)
		{
			return new VocaDbUrl($"{picture.EntryType}/{picture.Id}/{(size == ImageSize.Original ? "Picture" : "PictureThumb")}", UrlDomain.Main, UriKind.Relative);
		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size)
		{
			return new DynamicImageUrlFactory(UrlHelper).HasImage(picture, size);
		}

		public bool IsSupported(IEntryImageInformation picture, ImageSize size)
		{
			return new DynamicImageUrlFactory(UrlHelper).IsSupported(picture, size);
		}
	}
}
