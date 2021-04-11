#nullable disable


using System;

namespace VocaDb.Model.Domain
{
	public abstract class EntryHit : IEntryWithLongId
	{
		private int _agent;

		protected EntryHit() { }

		protected EntryHit(int agent)
		{
			Agent = agent;
		}

		public virtual int Agent
		{
			get => _agent;
			set => _agent = value;
		}

		/// <summary>
		/// Database-generated
		/// </summary>
		public virtual DateTime Date { get; set; }

		public virtual long Id { get; set; }
	}

	public class GenericEntryHit<TEntry> : EntryHit where TEntry : class
	{
		private TEntry _entry;

		public GenericEntryHit() { }
		public GenericEntryHit(TEntry entry, int agent) : base(agent)
		{
			Entry = entry;
		}

		public virtual TEntry Entry
		{
			get => _entry;
			set
			{
				ParamIs.NotNull(() => value);
				_entry = value;
			}
		}

#nullable enable
		public override string ToString()
		{
			return "Hit for " + Entry + " by " + Agent;
		}
#nullable disable
	}
}
