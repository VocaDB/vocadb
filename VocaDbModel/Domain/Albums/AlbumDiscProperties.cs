#nullable disable

using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Model.Domain.Albums
{
	public class AlbumDiscProperties : IEntryWithIntId
	{
		private Album album;
		private string name;

		public AlbumDiscProperties()
		{
			MediaType = DiscMediaType.Audio;
		}

		public AlbumDiscProperties(Album album, AlbumDiscPropertiesContract contract)
		{
			ParamIs.NotNull(() => album);

			Album = album;
			CopyContentFrom(contract);
		}

		public virtual Album Album
		{
			get { return album; }
			set
			{
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public virtual int DiscNumber { get; set; }

		public virtual int Id { get; set; }

		public virtual DiscMediaType MediaType { get; set; }

		public virtual string Name
		{
			get { return name; }
			set
			{
				ParamIs.NotNull(() => value);
				name = value;
			}
		}

		public virtual void CopyContentFrom(AlbumDiscPropertiesContract contract)
		{
			ParamIs.NotNull(() => contract);

			DiscNumber = contract.DiscNumber;
			MediaType = contract.MediaType;
			Name = contract.Name;
		}
	}

	public enum DiscMediaType
	{
		Audio,
		Video
	}
}
