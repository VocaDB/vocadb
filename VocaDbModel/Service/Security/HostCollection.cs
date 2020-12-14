#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VocaDb.Model.Service.Security
{
	public interface IHostCollection
	{
		IReadOnlyCollection<string> Hosts { get; }
		bool Contains(string host);
	}

	/// <summary>
	/// Thread-safe collection of hostnames.
	/// </summary>
	public class HostCollection : IHostCollection
	{
		private HashSet<string> ips;
		private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

		public HostCollection()
		{
			ips = new HashSet<string>();
		}

		public HostCollection(IEnumerable<string> ips)
		{
			this.ips = new HashSet<string>(ips);
		}

		public void Add(string host)
		{
			readerWriterLock.EnterWriteLock();
			try
			{
				ips.Add(host);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
		}

		public void Reset(IEnumerable<string> ips)
		{
			readerWriterLock.EnterWriteLock();
			try
			{
				this.ips = new HashSet<string>(ips);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
		}

		public bool Contains(string host)
		{
			readerWriterLock.EnterReadLock();
			try
			{
				return ips.Contains(host);
			}
			finally
			{
				readerWriterLock.ExitReadLock();
			}
		}

		public void Remove(string host)
		{
			readerWriterLock.EnterWriteLock();
			try
			{
				ips.Remove(host);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
		}

		public string[] Hosts
		{
			get
			{
				readerWriterLock.EnterReadLock();
				try
				{
					return ips.ToArray();
				}
				finally
				{
					readerWriterLock.ExitReadLock();
				}
			}
		}

		IReadOnlyCollection<string> IHostCollection.Hosts => Hosts;
	}
}