namespace VocaDb.Model.Domain
{
	public interface IOptionalDateTime
	{
		int? Day { get; }

		int? Month { get; }

		int? Year { get; }
	}
}