using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.BBCode {

	public static class BBElement {

		public static string GetCode(string tagName, string value) {

			return string.Format("[{0}]{1}[/{0}]", tagName, value);

		}

		public static string EntryLink(IEntryBase entry) {

			ParamIs.NotNull(() => entry);

			return GetCode(string.Format("{0}={1}", entry.EntryType, entry.Id), entry.DefaultName);

		}

	}

}
