using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Model.Domain.PVs
{

	/// <summary>
	/// PV that is editable
	/// </summary>
	public interface IEditablePV : IPV
	{

		void CopyMetaFrom(PVContract contract);

		void OnDelete();

	}

}
