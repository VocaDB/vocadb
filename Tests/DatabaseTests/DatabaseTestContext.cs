using System;
using System.Transactions;
using Autofac;
using NHibernate;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests {

	public class DatabaseTestContext<TTarget> {

		private IContainer Container => TestContainerManager.Container;

		public TResult RunTest<TResult>(Func<TTarget, TResult> func) {

			// Wrap inside transaction scope to make the test atomic
			using (new TransactionScope())
			using (var lifetimeScope = Container.BeginLifetimeScope()) {
				
				var target = lifetimeScope.Resolve<TTarget>();

				var result = func(target);

				DatabaseHelper.ClearSecondLevelCache(lifetimeScope.Resolve<ISessionFactory>());

				return result;

			}

		}

	}

	public class DatabaseTestContext : DatabaseTestContext<IDatabaseContext> { }

	public static class TestContainerManager {

		private static void EnsureContainerInitialized() {

			lock (containerLock) {
				if (container == null) {
					container = TestContainerFactory.BuildContainer();					
					testDatabase = container.Resolve<TestDatabase>();
				}
			}

		}

		private static IContainer container;
		private const string containerLock = "container";
		private static TestDatabase testDatabase;

		public static IContainer Container {
			get {
				EnsureContainerInitialized();
				return container;
			}
		}

		public static TestDatabase TestDatabase {
			get {
				EnsureContainerInitialized();
				return testDatabase;
			}
		}

	}

}
