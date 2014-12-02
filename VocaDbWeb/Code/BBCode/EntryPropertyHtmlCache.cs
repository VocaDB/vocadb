using System.Web.Mvc;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.BBCode;

namespace VocaDb.Web.Code.BBCode {

	public class EntryPropertyHtmlCache {

		private readonly BBCodeCache cache;

		private string Key(IEntryBase entry, string propertyName) {
			return string.Format("{0}.{1}.{2}", entry.EntryType, entry.Id, propertyName);
		}

		public EntryPropertyHtmlCache(BBCodeConverter converter) {
			cache = new BBCodeCache(converter);
		}

		public string GetHtml(IEntryBase entry, string propertyName, string rawValue) {

			var key = Key(entry, propertyName);
			return cache.GetHtml(rawValue, key);

		}
	
	}

}
