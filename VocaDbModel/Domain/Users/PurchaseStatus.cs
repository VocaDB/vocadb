using System;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Domain.Users
{

	public enum PurchaseStatus
	{

		Nothing = 0,

		Wishlisted = 1,

		Ordered = 2,

		Owned = 4

	}

	[Flags]
	public enum PurchaseStatuses
	{

		Nothing = PurchaseStatus.Nothing,

		Wishlisted = PurchaseStatus.Wishlisted,

		Ordered = PurchaseStatus.Ordered,

		Owned = PurchaseStatus.Owned,

		All = Wishlisted | Ordered | Owned

	}

	public static class PurchaseStatusesExtender
	{

		public static IEnumerable<PurchaseStatus> ToIndividualSelections(this PurchaseStatuses selections)
		{

			if (selections == PurchaseStatuses.Nothing)
				return new[] { PurchaseStatus.Nothing };

			return EnumVal<PurchaseStatuses>
				.GetIndividualValues(selections)
				.Where(s => s != PurchaseStatuses.All && s != PurchaseStatuses.Nothing)
				.Select(s => (PurchaseStatus)s);

		}

	}

}
