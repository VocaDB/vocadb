using System;
using System.Globalization;
using System.Threading;

namespace VocaDb.Model.Helpers {

	/// <summary>
	/// Temporarily sets UI Culture to something else.
	/// Use this in a using block, so that the Dispose method is called
	/// after exiting the block, resetting the culture.
	/// </summary>
	public class ImpersonateUICulture : IDisposable {

		private readonly CultureInfo old;

		public ImpersonateUICulture(CultureInfo impersonated) {
			old = Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentUICulture = impersonated;
		}

		public void Dispose() {
			Thread.CurrentThread.CurrentUICulture = old;
		}

	}
}
