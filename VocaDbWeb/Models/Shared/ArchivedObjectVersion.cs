using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Shared
{

	public class ArchivedObjectVersion
	{

		public static ArchivedObjectVersion Create(ArchivedObjectVersionContract contract, IEnumTranslations translator)
		{

			return new ArchivedObjectVersion(contract, contract.TranslateReason(translator),
				contract.TranslateChangedFields(translator), contract.IsAnythingChanged());

		}

		public ArchivedObjectVersion() { }

		public ArchivedObjectVersion(ArchivedObjectVersionContract contract, string reasonName, string changeMessage, bool anythingChanged = true)
		{

			Contract = contract;
			Hidden = contract.Hidden;
			Id = contract.Id;
			Notes = contract.Notes;
			Reason = reasonName;
			Status = contract.Status;
			ChangeMessage = changeMessage;
			AnythingChanged = anythingChanged;

		}

		public ArchivedObjectVersion(ArchivedObjectVersionContract contract, string changeMessage, bool anythingChanged = true)
		{

			Contract = contract;
			Hidden = contract.Hidden;
			Id = contract.Id;
			Notes = contract.Notes;
			Reason = Translate.EntryEditEventNames[contract.EditEvent];
			Status = contract.Status;
			ChangeMessage = changeMessage;
			AnythingChanged = anythingChanged;

		}

		public bool AnythingChanged { get; set; }

		public string ChangeMessage { get; set; }

		public ArchivedObjectVersionContract Contract { get; set; }

		public bool Hidden { get; set; }

		public int Id { get; set; }

		public string Notes { get; set; }

		public string Reason { get; set; }

		public EntryStatus Status { get; set; }

	}

}