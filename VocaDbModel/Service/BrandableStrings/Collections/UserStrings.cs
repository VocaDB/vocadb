#nullable disable

using System.Resources;

namespace VocaDb.Model.Service.BrandableStrings.Collections
{
	public class UserStrings : ResStringCollection
	{
		public UserStrings(ResourceManager resourceMan)
			: base(resourceMan) { }

		public string RequestVerificationInfo => GetString(nameof(RequestVerificationInfo));
	}
}
