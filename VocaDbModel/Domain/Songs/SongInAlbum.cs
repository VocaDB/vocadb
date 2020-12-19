#nullable disable

using System;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Domain.Songs
{
	public class SongInAlbum : ISongLink, IEntryWithIntId
	{
		private Album _album;
		private Song _song;

		public SongInAlbum() { }

		public SongInAlbum(Song song, Album album, int trackNumber, int discNumber)
		{
			Song = song;
			Album = album;
			TrackNumber = trackNumber;
			DiscNumber = discNumber;
		}

		public SongInAlbum(string name, Album album, int trackNumber, int discNumber)
		{
			Name = name;
			Album = album;
			TrackNumber = trackNumber;
			DiscNumber = discNumber;
		}

		public virtual int DiscNumber { get; set; }

		public virtual int Id { get; set; }

		/// <summary>
		/// Track index on the album.
		/// </summary>
		public virtual TrackIndex Index => new TrackIndex(DiscNumber, TrackNumber);

		public virtual string Name { get; set; }

		/// <summary>
		/// Song entry. 
		/// CAN BE NULL for custom songs.
		/// </summary>
		public virtual Song Song
		{
			get => _song;
			set => _song = value;
		}

		public virtual Album Album
		{
			get => _album;
			set
			{
				ParamIs.NotNull(() => value);
				_album = value;
			}
		}

		public virtual string SongToStringOrName => Song != null ? Song.ToString() : Name;

		public virtual int TrackNumber { get; set; }

		public virtual bool Equals(SongInAlbum another)
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
			Album.AllSongs.Remove(this);

			if (Song != null)
				Song.AllAlbums.Remove(this);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as SongInAlbum);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public virtual void Move(Album target)
		{
			ParamIs.NotNull(() => target);

			// Do nothing is target is the same as source
			if (target.Equals(Album))
				return;

			// Move track as the last one
			DiscNumber = target.LastDiscNumber;
			TrackNumber = target.GetNextTrackNumber(DiscNumber);

			Album.AllSongs.Remove(this);
			target.AllSongs.Add(this);
			Album = target;
		}

		public virtual void Move(Song target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Song))
				return;

			if (Song == null)
				throw new InvalidOperationException("Cannot move a track with no song to another song");

			Song.AllAlbums.Remove(this);
			target.AllAlbums.Add(this);
			Song = target;
		}

		public virtual void OnDeleting()
		{
			Album.OnSongDeleting(this);
		}

		public override string ToString()
		{
			return $"({DiscNumber}.{TrackNumber}) {SongToStringOrName} in {Album}";
		}
	}
}
