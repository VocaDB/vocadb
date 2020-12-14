#nullable disable

using System.Collections.Generic;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Domain
{
	public interface IEntryWithLinks<TLink> where TLink : WebLink
	{
		IList<TLink> WebLinks { get; }
	}
}
