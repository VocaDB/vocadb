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
		private HashSet<string> _ips;
		private readonly ReaderWriterLockSlim _readerWriterLock = new();

		public HostCollection()
		{
			_ips = new HashSet<string>();
		}

		public HostCollection(IEnumerable<string> ips)
		{
			this._ips = new HashSet<string>(ips);
		}

		public void Add(string host)
		{
			_readerWriterLock.EnterWriteLock();
			try
			{
				_ips.Add(host);
			}
			finally
			{
				_readerWriterLock.ExitWriteLock();
			}
		}

		public void Reset(IEnumerable<string> ips)
		{
			_readerWriterLock.EnterWriteLock();
			try
			{
				this._ips = new HashSet<string>(ips);
			}
			finally
			{
				_readerWriterLock.ExitWriteLock();
			}
		}

		public bool Contains(string host)
		{
			_readerWriterLock.EnterReadLock();
			try
			{
				return _ips.Contains(host);
			}
			finally
			{
				_readerWriterLock.ExitReadLock();
			}
		}

		public void Remove(string host)
		{
			_readerWriterLock.EnterWriteLock();
			try
			{
				_ips.Remove(host);
			}
			finally
			{
				_readerWriterLock.ExitWriteLock();
			}
		}

		public string[] Hosts
		{
			get
			{
				_readerWriterLock.EnterReadLock();
				try
				{
					return _ips.ToArray();
				}
				finally
				{
					_readerWriterLock.ExitReadLock();
				}
			}
		}

		IReadOnlyCollection<string> IHostCollection.Hosts => Hosts;
	}
}