#nullable disable

using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain
{
	public interface IEntryWithNames : IEntryBase
	{
		INameManager Names { get; }
	}

	public interface IEntryWithNames<TName> : IEntryWithNames where TName : LocalizedStringWithId
	{
		new INameManager<TName> Names { get; }
	}
}
