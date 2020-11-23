using System.Net;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.AlbumImport
{

	public class WebPictureDownloader : IPictureDownloader
	{

		public PictureDataContract Create(string url)
		{

			var request = WebRequest.Create(url);

			using (var response = (HttpWebResponse)request.GetResponse())
			{

				if (response.StatusCode == HttpStatusCode.NotFound)
					return null;

				using (var stream = response.GetResponseStream())
				{

					var buf = StreamHelper.ReadStream(stream, response.ContentLength);

					return new PictureDataContract(buf, response.ContentType);

				}

			}

		}

	}

}
