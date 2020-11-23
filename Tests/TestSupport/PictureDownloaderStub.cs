using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.AlbumImport;

namespace VocaDb.Tests.TestSupport
{
	public class PictureDownloaderStub : IPictureDownloader
	{
		public PictureDataContract Create(string url)
		{
			return new PictureDataContract(new byte[0], url); // Return something to show the method was called.
		}
	}
}
