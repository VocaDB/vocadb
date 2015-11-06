using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Web.Helpers {

	public static class ReportUtils {

		public static object GetReportTypes<TEnum>(TranslateableEnum<TEnum> translations, HashSet<TEnum> notesRequired) 
			where TEnum : struct, IConvertible {
			
			return translations.ValuesAndNames.Select(r => new {
				Id = r.Key,
				Name = r.Value,
				NotesRequired = notesRequired.Contains(r.Key)
			});

		}

	}

}