namespace VocaDb.Model.Helpers {

	public struct CollectionAddResult<T> {

		public CollectionAddResult(T result, bool isNew)
			: this() {
			
			Result = result;
			IsNew = isNew;

		} 

		public bool IsNew { get; private set; }

		public T Result { get; private set; }

	}

	public static class CollectionAddResult {
	
		public static CollectionAddResult<T> Create<T>(T result, bool isNew) {
			return new CollectionAddResult<T>(result, isNew);
		}
	
	}

}
