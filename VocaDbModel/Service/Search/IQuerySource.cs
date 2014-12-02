using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace VocaDb.Model.Service.Search {

	/// <summary>
	/// Interface for arbitrary typed LINQ queries to a data source.
	/// </summary>
	public interface IQuerySource {

		T Load<T>(object id);

		IQueryable<T> Query<T>();

	}

	/// <summary>
	/// Wraps NHibernate session into a query source.
	/// </summary>
	public class QuerySourceSession : IQuerySource {

		private readonly ISession session;

		public QuerySourceSession(ISession session) {
			this.session = session;
		}

		public T Load<T>(object id) {
			return session.Load<T>(id);
		}

		public IQueryable<T> Query<T>() {
			return session.Query<T>();
		}

	}

}
