using System.Globalization;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Helpers {

	public static class CultureHelper {

		public static CultureInfo Default => CultureInfo.InvariantCulture;

		/// <summary>
		/// Gets the culture with the specific name, or the application default culture (English basically).
		/// </summary>
		/// <param name="culture">Culture name, for example "en-US".</param>
		/// <returns>The specified culture, or application default culture. Cannot be null.</returns>
		public static CultureInfo GetCultureOrDefault(OptionalCultureCode culture) {

			return culture.GetCultureInfoSafe() ?? Default;

		}

	}
}
