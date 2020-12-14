#nullable disable

using System;

namespace VocaDb.Model.Domain.Web {

	public static class GlobalServerPathMapper {

		private static Func<IServerPathMapper> factory;

		public static void Configure(Func<IServerPathMapper> factory) {
			GlobalServerPathMapper.factory = factory;
		}

		public static IServerPathMapper ServerPathMapper => factory != null ? factory() : 
			throw new InvalidOperationException("Server path mapper factory is not configured");

	}

}
