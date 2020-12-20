#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Songs
{
	public class AlternateVersionForSong
	{
		private Song _derived;
		private Song _original;

		public AlternateVersionForSong() { }

		public AlternateVersionForSong(Song original, Song derived)
		{
			Original = original;
			Derived = derived;
		}

		public virtual Song Derived
		{
			get => _derived;
			set
			{
				ParamIs.NotNull(() => value);
				_derived = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual Song Original
		{
			get => _original;
			set
			{
				ParamIs.NotNull(() => value);
				_original = value;
			}
		}
	}
}
