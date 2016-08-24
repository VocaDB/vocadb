
namespace VocaDb.Model.Domain.Globalization {

	public struct LanguageCodeAndName {

		public LanguageCodeAndName(string code, string displayName) {
			Code = code;
			DisplayName = displayName;
		}

		public string Code { get; }

		public string DisplayName { get; }

	}

}
