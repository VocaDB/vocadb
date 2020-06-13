using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Songs {

	public interface ISongList {

		IUser Author { get; }

		bool FeaturedList { get; }

	}

}
