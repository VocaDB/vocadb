using System.Web.Routing;
using MvcPaging;

namespace VocaDb.Web.Models.Shared {

	/// <summary>
	/// Properties for AJAX paging using MvcPaging.
	/// </summary>
	public interface IPagingData {

		string Action { get; }

		/// <summary>
		/// Whether total number of items should be added to route (as totalCount parameter).
		/// </summary>
		bool AddTotalCount { get; }

		/// <summary>
		/// Name of the element containing the paged list.
		/// </summary>
		string ContainerName { get; }

		/// <summary>
		/// Object Id, to be added automatically to the route. If null, this will be skipped.
		/// </summary>
		object Id { get; }

		IPagedList ItemsBase { get; }

		/// <summary>
		/// List of additional route values. The common route values (action, totalCount and id) need not be specified.
		/// </summary>
		RouteValueDictionary RouteValues { get; }

	}

	public class PagingData<T> : IPagingData {

		public PagingData() { }

		public PagingData(IPagedList<T> items, object id, string action, string containerName, bool addTotalCount = false) {

			Items = items;
			Id = id;
			Action = action;
			ContainerName = containerName;
			AddTotalCount = addTotalCount;

		}

		public string Action { get; set; }

		public bool AddTotalCount { get; set; }

		public string ContainerName { get; set; }

		public object Id { get; set; }

		public IPagedList<T> Items { get; set; }

		public RouteValueDictionary RouteValues { get; set; } 

		public IPagedList ItemsBase {
			get { return Items; }
		}
	}
}