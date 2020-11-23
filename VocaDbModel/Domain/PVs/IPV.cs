using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Model.Domain.PVs
{

	/// <summary>
	/// Interface for PVs
	/// </summary>
	public interface IPV
	{

		bool Disabled { get; }

		/// <summary>
		/// Unique Id.
		/// </summary>
		int Id { get; }

		string PVId { get; }

		/// <summary>
		/// PV type
		/// </summary>
		PVType PVType { get; }

		/// <summary>
		/// PV service
		/// </summary>
		PVService Service { get; }

		bool ContentEquals(PVContract pv);

	}

}
