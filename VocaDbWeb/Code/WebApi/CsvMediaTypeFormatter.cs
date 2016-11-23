using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace VocaDb.Web.Code.WebApi {

	/// <summary>
	/// CSV media type formatter.
	/// Currently only supports dictionary of string.
	/// </summary>
	public class CsvMediaTypeFormatter : BufferedMediaTypeFormatter {

		public CsvMediaTypeFormatter() {
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
		}

		public override bool CanReadType(Type type) {
			return false;
		}

		public override bool CanWriteType(Type type) {

			return type == typeof(IEnumerable<Dictionary<string, string>>);

		}

		public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content) {

			using (var writer = new StreamWriter(writeStream)) {
				var lines = value as IEnumerable<Dictionary<string, string>>;
				if (lines != null) {
					foreach (var line in lines) {
						writer.WriteLine(string.Join(";", line.Values));
					}
				}
			}

		}
	}

}