namespace VocaDb.Model.Helpers;

public class LambdaComparer<T> : IComparer<T>
{
	private readonly Func<T?, T?, int> _comparer;

	public LambdaComparer(Func<T?, T?, int> comparer)
	{
		ParamIs.NotNull(() => comparer);
		_comparer = comparer;
	}

	public int Compare(T? x, T? y)
	{
		return _comparer(x, y);
	}
}
