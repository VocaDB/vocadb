using NHibernate;

namespace VocaDb.Model.Service.Helpers
{
	public static class DatabaseHelper
	{
		public static void ClearSecondLevelCache(ISessionFactory sessionFactory)
		{
			var classMetadata = sessionFactory.GetAllClassMetadata();
			foreach (var ep in classMetadata.Values)
			{
				sessionFactory.EvictEntity(ep.EntityName);
			}

			var collMetadata = sessionFactory.GetAllCollectionMetadata();
			foreach (var acp in collMetadata.Values)
			{
				sessionFactory.EvictCollection(acp.Role);
			}
		}
	}
}
