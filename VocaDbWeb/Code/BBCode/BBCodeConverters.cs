using VocaDb.Model.Service.BBCode;

namespace VocaDb.Web.Code.BBCode {

	public static class BBCodeConverters {

		public static BBCodeConverter Default() {

			return new BBCodeConverter(new[] { new AutoLinkTransformer() });

		}

	}

}