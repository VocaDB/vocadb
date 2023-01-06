#nullable disable


namespace VocaDb.Model.DataContracts.Versioning;

[Obsolete]
public class ComparedVersionsContract<T> where T : class
{
	public static ComparedVersionsContract<T> Create<TSource>(
		TSource firstData, TSource secondData, Func<TSource, T> dataGetter, Func<TSource, int> idGetter) where TSource : class
	{
		var firstId = idGetter(firstData);
		int secondId = 0;
		T secondDataRes = null;

		if (secondData != null)
		{
			secondId = idGetter(secondData);
			secondDataRes = dataGetter(secondData);
		}

		return new ComparedVersionsContract<T>(firstId, dataGetter(firstData), secondId, secondDataRes);
	}

	public ComparedVersionsContract(int firstId, T firstData, int secondId, T secondData)
	{
		FirstId = firstId;
		FirstData = firstData;
		SecondId = secondId;
		SecondData = secondData;
	}

	public ComparedVersionsContract(ComparedVersionsContract<T> another)
		: this(another.FirstId, another.FirstData, another.SecondId, another.SecondData) { }

	public int FirstId { get; init; }

	public T FirstData { get; init; }

	public int SecondId { get; init; }

	public T SecondData { get; init; }
}