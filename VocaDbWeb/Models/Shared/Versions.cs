using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Web.Models.Shared {

	public class Versions<TEntry> where TEntry : class {

		public Versions() { }

		public Versions(TEntry contract, IEnumerable<ArchivedObjectVersionContract> archivedVersions, IEnumTranslations translator) {

			ParamIs.NotNull(() => contract);

			Entry = contract;
			ArchivedVersions = archivedVersions.Select(t => ArchivedObjectVersion.Create(t, translator)).ToArray();

		}

		public Versions(IEntryWithArchivedVersionsContract<TEntry, ArchivedObjectVersionContract> contract, IEnumTranslations translator) {

			ParamIs.NotNull(() => contract);

			Entry = contract.Entry;
			ArchivedVersions = contract.ArchivedVersions.Select(t => ArchivedObjectVersion.Create(t, translator)).ToArray();

		}

		public ArchivedObjectVersion[] ArchivedVersions { get; set; }

		public TEntry Entry { get; set; }

	}

	/*public static class Versions {
		
		public static Versions<TEntry> Create<TEntry>(IEntryWithArchivedVersionsContract<TEntry, ArchivedObjectVersionContract> contract, IEnumTranslations translator) where TEntry : class {
			return new Versions<TEntry>(contract, translator);
		}

	}*/

}