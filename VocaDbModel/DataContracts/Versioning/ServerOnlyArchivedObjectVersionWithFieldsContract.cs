#nullable disable

using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.DataContracts.Versioning
{
	public class ServerOnlyArchivedObjectVersionWithFieldsContract<TFields, TReason>
		: ServerOnlyArchivedObjectVersionContract
		where TFields : struct, Enum
		where TReason : struct, Enum
	{
		private TFields DefaultField => default;

		public ServerOnlyArchivedObjectVersionWithFieldsContract() { }

		public ServerOnlyArchivedObjectVersionWithFieldsContract(ArchivedObjectVersion archivedVersion,
			TFields fields, TReason reason)
			: base(archivedVersion)
		{
			ChangedFields = fields;
			Reason = reason;
		}

		public TFields ChangedFields { get; init; }

		public TReason Reason { get; init; }

		public override bool IsAnythingChanged()
		{
			return !Equals(ChangedFields, DefaultField) || !Equals(Reason, default(TReason));
		}

		public override string TranslateChangedFields(IEnumTranslations translator)
		{
			return !Equals(ChangedFields, DefaultField) ? translator.Translations<TFields>().GetAllNameNames(ChangedFields, DefaultField) : string.Empty;
		}

		public override string TranslateReason(IEnumTranslations translator)
		{
			return !Equals(Reason, default(TReason)) ? translator.Translations<TReason>().GetName(Reason) : Notes;
		}
	}

	public static class ServerOnlyArchivedObjectVersionWithFieldsContract
	{
		public static ServerOnlyArchivedObjectVersionWithFieldsContract<TFields, TReason> Create<TFields, TReason>(
			ArchivedObjectVersion archivedVersion,
			TFields fields, TReason reason) where TFields : struct, Enum where TReason : struct, Enum
		{
			return new ServerOnlyArchivedObjectVersionWithFieldsContract<TFields, TReason>(archivedVersion, fields, reason);
		}
	}
}
