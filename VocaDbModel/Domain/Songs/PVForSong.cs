using System;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Domain.Songs {

	public class PVForSong : PV, IPVWithThumbnail, ISongLink {

		private Song song;
		private string thumbUrl;

		public PVForSong() {
			ThumbUrl = string.Empty;
		}

		public PVForSong(Song song, PVContract contract)
			: base(contract) {

			Song = song;
			Length = contract.Length;
			ThumbUrl = contract.ThumbUrl ?? string.Empty;

		}

		public virtual int Length { get; set; }

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual string ThumbUrl {
			get { return thumbUrl; }
			set { 
				ParamIs.NotNull(() => value);
				thumbUrl = value; 
			}
		}

		public override void CopyMetaFrom(PVContract contract) {

			base.CopyMetaFrom(contract);

			ThumbUrl = contract.ThumbUrl;

		}

		public virtual bool Equals(PVForSong another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as PVForSong);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override void OnDelete() {

			Song.PVs.Remove(this);
			Song.UpdateNicoId();
			Song.UpdatePVServices();

		}

		public override string ToString() {
			return string.Format("PV '{0}' [{1}] for {2}", PVId, Id, Song);
		}

	}
}
