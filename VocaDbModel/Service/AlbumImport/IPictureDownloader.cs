#nullable disable

using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Service.AlbumImport
{
	public interface IPictureDownloader
	{
		PictureDataContract Create(string url);
	}
}
