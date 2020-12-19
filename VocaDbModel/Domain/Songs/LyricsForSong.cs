#nullable disable

using System;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Model.Domain.Songs
{
	public class LyricsForSong : IEquatable<LyricsForSong>, IDatabaseObject
	{
		private OptionalCultureCode _cultureCode;
		private string _notes;
		private Song _song;
		private string _source;
		private string _value;
		private string _url;

		public LyricsForSong() { }

		public LyricsForSong(Song song, string val, string source, string url, TranslationType translationType, string cultureCode)
		{
			Song = song;
			Source = source;
			URL = url;
			TranslationType = translationType;
			CultureCode = new OptionalCultureCode(cultureCode);
			Value = val;
		}

		public virtual OptionalCultureCode CultureCode
		{
			get => _cultureCode ?? (_cultureCode = OptionalCultureCode.Empty);
			set
			{
				_cultureCode = value ?? OptionalCultureCode.Empty;
			}
		}

		public virtual int Id { get; protected set; }

		public virtual string Notes
		{
			get => _notes;
			set
			{
				ParamIs.NotNull(() => value);
				_notes = value;
			}
		}

		public virtual Song Song
		{
			get => _song;
			set
			{
				ParamIs.NotNull(() => value);
				_song = value;
			}
		}

		public virtual string Source
		{
			get => _source;
			set
			{
				ParamIs.NotNull(() => value);
				_source = value;
			}
		}

		public virtual TranslationType TranslationType { get; set; }

		public virtual string URL
		{
			get => _url;
			set
			{
				ParamIs.NotNull(() => value);
				_url = value;
			}
		}

		public virtual string Value
		{
			get => _value;
			set
			{
				ParamIs.NotNull(() => value);
				this._value = value;
			}
		}

		public virtual bool ContentEquals(LyricsForSongContract contract)
		{
			if (contract == null)
				return false;

			return (TranslationType == contract.TranslationType
				&& CultureCode.CultureCode == contract.CultureCode
				&& Source == contract.Source
				&& URL == contract.URL
				&& Value == contract.Value);
		}

		public virtual bool Equals(LyricsForSong another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as LyricsForSong);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
