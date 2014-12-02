using System.IO;
using Newtonsoft.Json;

namespace VocaDb.Model.EmbeddedResources {

	public static class EmbeddedResourceHelper {

		public static Stream GetFileStream(string fileName) {

			var asm = typeof(EmbeddedResourceHelper).Assembly;
			var s = asm.GetManifestResourceNames();
			return asm.GetManifestResourceStream(string.Format("{0}.EmbeddedResources.{1}", asm.GetName().Name, fileName));

		}

		public static string ReadTextFile(string fileName) {

			using (var stream = GetFileStream(fileName))
			using (var reader = new StreamReader(stream)) {

				return reader.ReadToEnd();

			}

		}

		public static T ReadJson<T>(string fileName) {
			
			return JsonConvert.DeserializeObject<T>(ReadTextFile(fileName));

		}

	}

}
