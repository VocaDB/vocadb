using System.Collections.Generic;
using VocaDb.Model;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain.MikuDb;

namespace VocaDb.Web.Models.MikuDbAlbums {

	public class Index {

		public Index() {}

		public Index(MikuDbAlbumContract[] albums, string titleFilter, AlbumStatus status) {

			ParamIs.NotNull(() => albums);

			Albums = albums;
			AllStatuses = new Dictionary<AlbumStatus, string> { { AlbumStatus.New, "Unprocessed (new)" }, { AlbumStatus.Skipped, "Rejected" }, { AlbumStatus.Approved, "Processed" } };
			Status = status;
			TitleFilter = titleFilter;

		}

		public MikuDbAlbumContract[] Albums { get; set; }

		public Dictionary<AlbumStatus, string> AllStatuses { get; set; }

		public AlbumStatus Status { get; set; }

		public string TitleFilter { get; set; }

	}

}