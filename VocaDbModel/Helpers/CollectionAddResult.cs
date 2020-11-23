namespace VocaDb.Model.Helpers
{
	public readonly struct CollectionAddResult<T>
	{
		public CollectionAddResult(T result, bool isNew)
			: this()
		{
			Result = result;
			IsNew = isNew;
		}

		public bool IsNew { get; }

		public T Result { get; }
	}

	public static class CollectionAddResult
	{
		public static CollectionAddResult<T> Create<T>(T result, bool isNew) => new CollectionAddResult<T>(result, isNew);
	}
}
