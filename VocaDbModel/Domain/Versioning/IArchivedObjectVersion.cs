namespace VocaDb.Model.Domain.Versioning {

	public interface IArchivedObjectVersion : IDatabaseObject {

		bool Hidden { get; set; }

	}

}
