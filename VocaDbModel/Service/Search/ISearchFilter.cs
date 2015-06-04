using System.Collections.Generic;
using System.Linq;
using NHibernate;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Search {

	public interface ISearchFilter<TEntry> {

		QueryCost Cost { get; }

		//void FilterResults(List<TEntry> albums, ISession session);

		//List<TEntry> GetResults(ISession session);

		IQueryable<TEntry> Filter(IQueryable<TEntry> query, IRepositoryContext session);

		IQueryable<TEntry> Query(IRepositoryContext session);

	}

}
