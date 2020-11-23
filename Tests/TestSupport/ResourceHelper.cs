using System.IO;
using System.Text;
using HtmlAgilityPack;

namespace VocaDb.Tests.TestSupport
{

	public static class ResourceHelper
	{

		public static Stream GetFileStream(string fileName)
		{

			var asm = typeof(ResourceHelper).Assembly;
			var s = asm.GetManifestResourceNames();
			return asm.GetManifestResourceStream(asm.GetName().Name + ".TestData." + fileName);

		}

		public static string ReadTextFile(string fileName)
		{

			using (var stream = GetFileStream(fileName))
			using (var reader = new StreamReader(stream))
			{

				return reader.ReadToEnd();

			}

		}

		public static HtmlDocument ReadHtmlDocument(string fileName, Encoding encoding = null)
		{

			using (var stream = GetFileStream(fileName))
			{

				var doc = new HtmlDocument();
				doc.Load(stream, encoding ?? Encoding.Default);
				return doc;

			}

		}

		public static Stream TestImage()
		{
			return GetFileStream("yokohma_bay_concert.jpg");
		}

		public static Stream TestImage2 => GetFileStream("vocadb_favicon.png");

	}

}
