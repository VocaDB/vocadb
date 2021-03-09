#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts.UseCases
{
	public class EntryRevertedContract
	{
		public EntryRevertedContract(IEntryBase entryBase, IEnumerable<string> warnings)
		{
			ParamIs.NotNull(() => entryBase);
			ParamIs.NotNull(() => warnings);

			Id = entryBase.Id;
			Warnings = warnings.ToArray();
		}

		public int Id { get; init; }

		public string[] Warnings { get; init; }
	}
}
