using System;
using System.Text;

namespace VocaDb.Web.Helpers.Support {

	public class CSSClassBuilder<T> where T : class {

		private readonly T model;
		private readonly StringBuilder str;

		public CSSClassBuilder(T model, string initial) {
			this.model = model;
			str = new StringBuilder(initial);
		}

		public CSSClassBuilder<T> If(bool predicate, string className) {

			if (predicate)
				str.Append(" " + className);

			return this;

		}

		public CSSClassBuilder<T> If(Func<T, bool> func, string className) {

			if (func(model))
				str.Append(" " + className);

			return this;

		}

		public override string ToString() {
			return str.ToString();
		}

	}
}