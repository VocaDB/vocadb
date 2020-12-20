#nullable disable

using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace VocaDb.Tests.TestSupport
{
	public class FakeObjectCache : ObjectCache
	{
		public override bool Add(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
		{
			return true;
		}

		public override bool Add(string key, object value, CacheItemPolicy policy, string regionName = null)
		{
			return true;
		}

		public override bool Add(CacheItem item, CacheItemPolicy policy)
		{
			return true;
		}

		public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public override bool Contains(string key, string regionName = null)
		{
			return false;
		}

		public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
		{
			throw new NotImplementedException();
		}

		public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
		{
			throw new NotImplementedException();
		}

		public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null)
		{
			throw new NotImplementedException();
		}

		public override object Get(string key, string regionName = null)
		{
			return null;
		}

		public override CacheItem GetCacheItem(string key, string regionName = null)
		{
			throw new NotImplementedException();
		}

		public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
		{
			throw new NotImplementedException();
		}

		public override void Set(CacheItem item, CacheItemPolicy policy)
		{
			throw new NotImplementedException();
		}

		public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
		{
			throw new NotImplementedException();
		}

		public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
		{
			throw new NotImplementedException();
		}

		public override object Remove(string key, string regionName = null)
		{
			throw new NotImplementedException();
		}

		public override long GetCount(string regionName = null)
		{
			throw new NotImplementedException();
		}

		public override DefaultCacheCapabilities DefaultCacheCapabilities
		{
			get { throw new NotImplementedException(); }
		}

		public override string Name
		{
			get { throw new NotImplementedException(); }
		}

		public override object this[string key]
		{
			get { throw new NotImplementedException(); }
			set => throw new NotImplementedException();
		}
	}
}
