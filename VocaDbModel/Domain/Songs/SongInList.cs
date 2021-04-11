#nullable disable

namespace VocaDb.Model.Domain.Songs
{
	public class SongInList : IEntryWithIntId, ISongLink
	{
		private SongList _list;
		private string _notes;
		private Song _song;

		public SongInList()
		{
			Notes = string.Empty;
		}

		public SongInList(Song song, SongList list, int order, string notes)
			: this()
		{
			Song = song;
			List = list;
			Order = order;
			Notes = notes;
		}

		public virtual int Id { get; set; }

		public virtual Song Song
		{
			get => _song;
			set
			{
				ParamIs.NotNull(() => value);
				_song = value;
			}
		}

		public virtual SongList List
		{
			get => _list;
			set
			{
				ParamIs.NotNull(() => value);
				_list = value;
			}
		}

		public virtual string Notes
		{
			get => _notes;
			set
			{
				ParamIs.NotNull(() => value);
				_notes = value;
			}
		}

		public virtual int Order { get; set; }

#nullable enable
		public virtual void ChangeSong(Song target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Song))
				return;

			Song.ListLinks.Remove(this);
			target.ListLinks.Add(this);
			Song = target;
		}

		public virtual bool Equals(SongInList? another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return Id == another.Id;
		}
#nullable disable

		public virtual void Delete()
		{
			List.AllSongs.Remove(this);
			Song.ListLinks.Remove(this);
		}

#nullable enable
		public override bool Equals(object? obj)
		{
			return Equals(obj as SongInList);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Song} in {List}";
		}
#nullable disable
	}
}
