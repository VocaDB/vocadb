using System;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.DataContracts.Versioning {

	public class ArchivedObjectVersionWithFieldsContract<TFields, TReason>
		: ArchivedObjectVersionContract 
		where TFields : struct, IConvertible 
		where TReason : struct, IConvertible {

		private TFields DefaultField => default(TFields);

		public ArchivedObjectVersionWithFieldsContract() { }

		public ArchivedObjectVersionWithFieldsContract(ArchivedObjectVersion archivedVersion,
			TFields fields, TReason reason)
			: base(archivedVersion) {

			ChangedFields = fields;
			Reason = reason;

		}

		public TFields ChangedFields { get; set; }

		public TReason Reason { get; set; }

		public override bool IsAnythingChanged() {
			return !Equals(ChangedFields, DefaultField) || !Equals(Reason, default(TReason));
		}

		public override string TranslateChangedFields(IEnumTranslations translator) {
			return !Equals(ChangedFields, DefaultField) ? translator.Translations<TFields>().GetAllNameNames(ChangedFields, DefaultField) : string.Empty;
		}

		public override string TranslateReason(IEnumTranslations translator) {
			return !Equals(Reason, default(TReason)) ? translator.Translations<TReason>().GetName(Reason) : Notes;
		}

	}

	public static class ArchivedObjectVersionWithFieldsContract {
		
		public static ArchivedObjectVersionWithFieldsContract<TFields, TReason> Create<TFields, TReason>(
			ArchivedObjectVersion archivedVersion,
			TFields fields, TReason reason) where TFields : struct, IConvertible where TReason : struct, IConvertible {
			return new ArchivedObjectVersionWithFieldsContract<TFields, TReason>(archivedVersion, fields, reason);
		}

	}

}
