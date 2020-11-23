using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace VocaDb.Model.Helpers
{
	/// <summary>
	/// Various helper methods for XML processing.
	/// </summary>
	public static class XmlHelper
	{
		public static string GetNodeTextOrEmpty(XElement node)
		{
			if (node == null)
				return string.Empty;

			return node.Value;
		}

		public static string GetNodeTextOrEmpty(XDocument doc, string xpath)
		{
			return GetNodeTextOrEmpty(doc.XPathSelectElement(xpath));
		}

		/// <summary>
		/// Serializes an object to a string in UTF-8 format, 
		/// including the XML declaration.
		/// </summary>
		/// <param name="doc">XML document to be serialized. Cannot be null.</param>
		/// <returns>Document serialized into XML string, in UTF-8 encoding.</returns>
		public static string SerializeToUTF8XmlString(XDocument doc)
		{
			ParamIs.NotNull(() => doc);

			doc.Declaration = new XDeclaration("1.0", "utf-8", "yes");

			using (var stream = new MemoryStream())
			{
				doc.Save(stream);
				stream.Seek(0, SeekOrigin.Begin);

				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		/// <summary>
		/// Serializes an object to a string in UTF-8 format, 
		/// including the XML declaration.
		/// </summary>
		/// <typeparam name="T">Type of the object to be serialized.</typeparam>
		/// <param name="obj">Object to be serialized. Cannot be null.</param>
		/// <returns>Object serialized into XML string, in UTF-8 encoding.</returns>
		public static string SerializeToUTF8XmlString<T>(T obj)
		{
			var doc = SerializeToXml(obj);
			return SerializeToUTF8XmlString(doc);
		}

		/// <summary>
		/// Serializes an object into XML.
		/// </summary>
		/// <typeparam name="T">Type of the object to be serialized.</typeparam>
		/// <param name="obj">The object to be serialized.</param>
		/// <returns>The object serialized as XML document. Cannot be null.</returns>
		/// <exception cref="XmlException">If the serialization failed. This could happen if the object contains illegal characters.</exception>
		/// <remarks>Some illegal characters are sanitized from the object, for example 0x02 (STX).</remarks>
		public static XDocument SerializeToXml<T>(T obj)
		{
			var serializer = new XmlSerializer(typeof(T));
			XDocument doc;

			using (var writer = new StringWriter())
			{
				serializer.Serialize(writer, obj);
				var str = StringHelper.CleanInvalidXmlChars(writer.ToString());
				doc = XDocument.Parse(str);
			}

			// More efficient, but doesn't support cleaning invalid chars.
			/*using (var stream = new MemoryStream()) {
				serializer.Serialize(stream, obj);
				stream.Seek(0, SeekOrigin.Begin);
				doc = XDocument.Load(stream);
			}*/

			/*var doc = new XDocument();
			serializer.Serialize(doc.CreateWriter(), obj);*/

			return doc;
		}

		/// <summary>
		/// Deserializes an object from XML.
		/// </summary>
		/// <typeparam name="T">Type of the object to be deserialized.</typeparam>
		/// <param name="doc">XML document containing the serialized object. Cannot be null.</param>
		/// <returns>The deserialized object.</returns>
		public static T DeserializeFromXml<T>(XDocument doc)
		{
			ParamIs.NotNull(() => doc);

			var serializer = new XmlSerializer(typeof(T));
			T obj;

			using (var reader = doc.CreateReader())
			{
				obj = (T)serializer.Deserialize(reader);
			}

			return obj;
		}
	}
}
