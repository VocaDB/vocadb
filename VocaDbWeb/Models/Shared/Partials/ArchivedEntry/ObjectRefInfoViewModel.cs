using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{

	public class ObjectRefInfoViewModel
	{

		public ObjectRefInfoViewModel(ObjectRefContract objRef)
		{
			ObjRef = objRef;
		}

		public ObjectRefContract ObjRef { get; set; }

	}

}