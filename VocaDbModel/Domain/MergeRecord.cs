namespace VocaDb.Model.Domain
{
	/// <summary>
	/// Record of one entry being merged to another.
	/// </summary>
	/// <typeparam name="T">Type of entry being merged.</typeparam>
	/// <remarks>
	/// Merge record is saved separately from the entry itself so it can be made into a weak link
	/// and the original entry deleted.
	/// </remarks>
	public abstract class MergeRecord<T> : IEntryWithIntId
		where T : class, IEntryBase
	{
		private T target;

		protected MergeRecord() { }

		protected MergeRecord(T source, T target)
		{
			ParamIs.NotNull(() => source);
			ParamIs.NotNull(() => target);

			this.Source = source.Id;
			this.Target = target;
		}

		public virtual int Id { get; set; }

		/// <summary>
		/// ID of the source entry that was merged.
		/// This entry might be permanently removed from the DB.
		/// </summary>
		public virtual int Source { get; set; }

		/// <summary>
		/// Target where the entry was merged to.
		/// At the moment, targets of merges cannot be deleted.
		/// In the future, this reference might be made weak as well.
		/// </summary>
		public virtual T Target
		{
			get { return target; }
			set
			{
				ParamIs.NotNull(() => value);
				target = value;
			}
		}
	}
}
