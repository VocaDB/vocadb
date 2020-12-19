#nullable disable

using System;

namespace VocaDb.Model.Domain.Web
{
	public static class GlobalServerPathMapper
	{
		private static Func<IServerPathMapper> _factory;

		public static void Configure(Func<IServerPathMapper> factory)
		{
			GlobalServerPathMapper._factory = factory;
		}

		public static IServerPathMapper ServerPathMapper => _factory != null ? _factory() :
			throw new InvalidOperationException("Server path mapper factory is not configured");
	}
}
