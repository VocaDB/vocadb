using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.DataContracts.Versioning {

	public class ArchivedObjectVersionWithFieldsContract<TFields, TReason> 
		: ArchivedObjectVersionContract {

		public ArchivedObjectVersionWithFieldsContract() { }

		public ArchivedObjectVersionWithFieldsContract(ArchivedObjectVersion archivedVersion,
			TFields fields, TReason reason)
			: base(archivedVersion) {

			ChangedFields = fields;
			Reason = reason;

		}

		public TFields ChangedFields { get; set; }

		public TReason Reason { get; set; }

	}

}
