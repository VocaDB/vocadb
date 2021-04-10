#nullable disable

using System;
using System.Xml.Linq;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.MikuDb
{
	public class MikuDbAlbum
	{
		private XDocument _data;
		private string _sourceUrl;
		private string _title;

		public MikuDbAlbum()
		{
			Created = DateTime.Now;
		}

#nullable enable
		public MikuDbAlbum(MikuDbAlbumContract contract)
			: this()
		{
			ParamIs.NotNull(() => contract);

			CoverPicture = (contract.CoverPicture != null ? new PictureData(contract.CoverPicture) : null);
			CoverPictureMime = (contract.CoverPicture != null ? contract.CoverPicture.Mime : null);
			Data = XmlHelper.SerializeToXml(contract.Data);
			SourceUrl = contract.SourceUrl;
			Status = contract.Status;
			Title = contract.Title;
		}
#nullable disable

		public virtual PictureData CoverPicture { get; set; }

		public virtual string CoverPictureMime { get; set; }

		public virtual DateTime Created { get; set; }

		public virtual XDocument Data
		{
			get => _data;
			set => _data = value;
		}

		public virtual int Id { get; set; }

		//public virtual ContentLanguageSelection Language { get; set; }

		public virtual string SourceUrl
		{
			get => _sourceUrl;
			set => _sourceUrl = value;
		}

		public virtual AlbumStatus Status { get; set; }

		public virtual string Title
		{
			get => _title;
			set => _title = value;
		}

#nullable enable
		public virtual bool Equals(MikuDbAlbum? another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return Id == another.Id;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as MikuDbAlbum);
		}
#nullable disable

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return $"Imported album '{Title}' [{Id}]";
		}
	}
}
