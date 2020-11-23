using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Songs
{
	public class AlternateVersionForSong
	{
		private Song derived;
		private Song original;

		public AlternateVersionForSong() { }

		public AlternateVersionForSong(Song original, Song derived)
		{
			Original = original;
			Derived = derived;
		}

		public virtual Song Derived
		{
			get { return derived; }
			set
			{
				ParamIs.NotNull(() => value);
				derived = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual Song Original
		{
			get { return original; }
			set
			{
				ParamIs.NotNull(() => value);
				original = value;
			}
		}
	}
}
