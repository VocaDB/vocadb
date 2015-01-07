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

	public static class ArchivedObjectVersionWithFieldsContract {
		
		public static ArchivedObjectVersionWithFieldsContract<TFields, TReason> Create<TFields, TReason>(
			ArchivedObjectVersion archivedVersion,
			TFields fields, TReason reason) {
			return new ArchivedObjectVersionWithFieldsContract<TFields, TReason>(archivedVersion, fields, reason);
		}

	}

}
