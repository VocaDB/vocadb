namespace VocaDb.Model.Domain.Songs
{
	public class SongInList : IEntryWithIntId, ISongLink
	{
		private SongList list;
		private string notes;
		private Song song;

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
			get { return song; }
			set
			{
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual SongList List
		{
			get { return list; }
			set
			{
				ParamIs.NotNull(() => value);
				list = value;
			}
		}

		public virtual string Notes
		{
			get { return notes; }
			set
			{
				ParamIs.NotNull(() => value);
				notes = value;
			}
		}

		public virtual int Order { get; set; }

		public virtual void ChangeSong(Song target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Song))
				return;

			Song.ListLinks.Remove(this);
			target.ListLinks.Add(this);
			Song = target;
		}

		public virtual bool Equals(SongInList another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;
		}

		public virtual void Delete()
		{
			List.AllSongs.Remove(this);
			Song.ListLinks.Remove(this);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as SongInList);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0} in {1}", Song, List);
		}
	}
}
