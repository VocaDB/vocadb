using System;
using VocaDb.Model.Domain.MikuDb;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.MikuDb
{
	public class MikuDbAlbumContract
	{
		public MikuDbAlbumContract()
		{
			SourceUrl = Title = string.Empty;
		}

		public MikuDbAlbumContract(MikuDbAlbum album)
			: this()
		{
			ParamIs.NotNull(() => album);

			Created = album.Created;
			Data = XmlHelper.DeserializeFromXml<ImportedAlbumDataContract>(album.Data);
			Id = album.Id;
			SourceUrl = album.SourceUrl;
			Status = album.Status;
			Title = album.Title;

			Selected = (Status == AlbumStatus.New);
		}

		public MikuDbAlbumContract(ImportedAlbumDataContract data)
			: this()
		{
			ParamIs.NotNull(() => data);

			Data = data;
			Created = DateTime.Now;
			Title = data.Title;
		}

		public PictureDataContract CoverPicture { get; set; }

		public DateTime Created { get; set; }

		public ImportedAlbumDataContract Data { get; set; }

		public int Id { get; set; }

		public bool Selected { get; set; }

		public string SourceUrl { get; set; }

		public AlbumStatus Status { get; set; }

		public string Title { get; set; }
	}
}
