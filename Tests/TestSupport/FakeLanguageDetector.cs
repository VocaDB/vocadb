using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Tests.TestSupport {

	public class FakeLanguageDetector : ILanguageDetector {

		public ContentLanguageSelection Detect(string str, ContentLanguageSelection def) {

			return def;

		}

	}

}
