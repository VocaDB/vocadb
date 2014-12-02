using System.Collections.Generic;
using System.Linq;
using NHibernate;

namespace VocaDb.Model.Service.Search {

	public interface ISearchFilter<TEntry> {

		QueryCost Cost { get; }

		//void FilterResults(List<TEntry> albums, ISession session);

		//List<TEntry> GetResults(ISession session);

		IQueryable<TEntry> Filter(IQueryable<TEntry> query, IQuerySource session);

		IQueryable<TEntry> Query(IQuerySource session);

	}

}
