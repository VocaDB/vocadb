#nullable disable

using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.AlbumImport;

namespace VocaDb.Tests.TestSupport;

public class PictureDownloaderStub : IPictureDownloader
{
	public PictureDataContract Create(string url)
	{
		return new PictureDataContract(Array.Empty<byte>(), url); // Return something to show the method was called.
	}
}
