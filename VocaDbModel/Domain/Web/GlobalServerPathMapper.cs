#nullable disable

using System;

namespace VocaDb.Model.Domain.Web
{
	public static class GlobalServerPathMapper
	{
		private static Func<IServerPathMapper> s_factory;

		public static void Configure(Func<IServerPathMapper> factory)
		{
			GlobalServerPathMapper.s_factory = factory;
		}

		public static IServerPathMapper ServerPathMapper => s_factory != null ? s_factory() :
			throw new InvalidOperationException("Server path mapper factory is not configured");
	}
}
