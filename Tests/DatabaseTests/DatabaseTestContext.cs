#nullable disable

using System;
using System.Threading.Tasks;
using System.Transactions;
using Autofac;
using NHibernate;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests
{
	public class DatabaseTestContext<TTarget>
	{
		private IContainer Container => TestContainerManager.Container;

		public void RunTest(Action<TTarget> func)
		{
			// Make sure session factory is built outside of transaction
			Container.Resolve<ISessionFactory>();

			// Wrap inside transaction scope to make the test atomic
			using (new TransactionScope())
			using (var lifetimeScope = Container.BeginLifetimeScope())
			{
				var target = lifetimeScope.Resolve<TTarget>();

				func(target);

				DatabaseHelper.ClearSecondLevelCache(lifetimeScope.Resolve<ISessionFactory>());
			}
		}

		public async Task RunTestAsync(Func<TTarget, Task> func)
		{
			// Make sure session factory is built outside of transaction
			Container.Resolve<ISessionFactory>();

			// Wrap inside transaction scope to make the test atomic
			using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			using (var lifetimeScope = Container.BeginLifetimeScope())
			{
				var target = lifetimeScope.Resolve<TTarget>();

				await func(target);

				DatabaseHelper.ClearSecondLevelCache(lifetimeScope.Resolve<ISessionFactory>());
			}
		}

		public TResult RunTest<TResult>(Func<TTarget, TResult> func)
		{
			// Make sure session factory is built outside of transaction
			Container.Resolve<ISessionFactory>();

			// Wrap inside transaction scope to make the test atomic
			using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			using (var lifetimeScope = Container.BeginLifetimeScope())
			{
				var target = lifetimeScope.Resolve<TTarget>();

				var result = func(target);

				DatabaseHelper.ClearSecondLevelCache(lifetimeScope.Resolve<ISessionFactory>());

				return result;
			}
		}

		public async Task<TResult> RunTestAsync<TResult>(Func<TTarget, Task<TResult>> func)
		{
			// Make sure session factory is built outside of transaction
			Container.Resolve<ISessionFactory>();

			// Wrap inside transaction scope to make the test atomic
			using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			using (var lifetimeScope = Container.BeginLifetimeScope())
			{
				var target = lifetimeScope.Resolve<TTarget>();

				var result = await func(target);

				DatabaseHelper.ClearSecondLevelCache(lifetimeScope.Resolve<ISessionFactory>());

				return result;
			}
		}
	}

	public class DatabaseTestContext : DatabaseTestContext<IDatabaseContext> { }

	public static class TestContainerManager
	{
		private static void EnsureContainerInitialized()
		{
			lock (ContainerLock)
			{
				if (_container == null)
				{
					_container = TestContainerFactory.BuildContainer();
					_testDatabase = _container.Resolve<TestDatabase>();
				}
			}
		}

		private static IContainer _container;
		private const string ContainerLock = "container";
		private static TestDatabase _testDatabase;

		public static IContainer Container
		{
			get
			{
				EnsureContainerInitialized();
				return _container;
			}
		}

		public static TestDatabase TestDatabase
		{
			get
			{
				EnsureContainerInitialized();
				return _testDatabase;
			}
		}
	}
}
