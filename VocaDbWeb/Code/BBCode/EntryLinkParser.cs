using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.Domain;
using VocaDb.Model;
using VocaDb.Model.Service.BBCode;

namespace VocaDb.Web.Code.EntryLinks {

	public class EntryLinkParser {

		public static string GenerateEntryLink(IEntryBase entry) {

			ParamIs.NotNull(() => entry);

			return BBElement.GetCode(string.Format("{0}={1}", entry.EntryType, entry.Id), entry.DefaultName);

		}

	}
}