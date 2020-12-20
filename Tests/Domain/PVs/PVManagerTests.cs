#nullable disable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Tests.Domain.PVs
{
	/// <summary>
	/// Tests for <see cref="PVManager{T}"/>.
	/// </summary>
	[TestClass]
	public class PVManagerTests
	{
		private PVManager<PVForSong> _manager;
		private PVContract _pvContract;
		private PVContract _pvContract2;

		private PVForSong CreatePV(PVContract contract)
		{
			return new PVForSong(new Song(), contract) { Id = contract.Id };
		}

		[TestInitialize]
		public void SetUp()
		{
			_manager = new PVManager<PVForSong>();
			_pvContract = new PVContract { Id = 1, Author = "Miku!", Name = "A cool Miku PV", PVId = "3939", ThumbUrl = "http://youtube.com/thumb", Url = "http://youtube.com/39" };
			_pvContract2 = new PVContract { Id = 2, Author = "Luka!", Name = "A cool Luka PV", PVId = "0303", ThumbUrl = "http://youtube.com/thumb3", Url = "http://youtube.com/3" };
		}

		[TestMethod]
		public void Preconditions()
		{
			Assert.IsFalse(_pvContract.ContentEquals(_pvContract2), "PVContracts are not equal");
		}

		[TestMethod]
		public void Sync_Contracts_NoExistingLinks()
		{
			var newPVs = new[] { _pvContract };

			var result = _manager.Sync(newPVs, CreatePV);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(1, result.Added.Length, "1 added");
			Assert.AreEqual(0, result.Edited.Length, "none edited");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(0, result.Unchanged.Length, "none unchanged");
			Assert.IsTrue(result.Added.First().ContentEquals(_pvContract), "added PV matches contract");
		}

		[TestMethod]
		public void Sync_Contracts_NotChanged()
		{
			_manager.PVs.Add(CreatePV(_pvContract));
			var newLinks = new[] { _pvContract };

			var result = _manager.Sync(newLinks, CreatePV);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsFalse(result.Changed, "is not changed");
			Assert.AreEqual(0, result.Added.Length, "none added");
			Assert.AreEqual(0, result.Edited.Length, "none edited");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(1, result.Unchanged.Length, "1 unchanged");
			Assert.IsTrue(result.Unchanged.First().ContentEquals(_pvContract), "unchanged PV matches contract");
		}

		[TestMethod]
		public void Sync_Contracts_Edited()
		{
			var oldPV = CreatePV(_pvContract);
			oldPV.Id = 2;
			_manager.PVs.Add(oldPV);
			var newLinks = new[] { _pvContract2 };

			var result = _manager.Sync(newLinks, CreatePV);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(0, result.Added.Length, "none added");
			Assert.AreEqual(1, result.Edited.Length, "1 edited");
			Assert.AreEqual(0, result.Removed.Length, "none removed");
			Assert.AreEqual(1, result.Unchanged.Length, "1 unchanged");
			Assert.IsTrue(result.Edited.First().ContentEquals(_pvContract2), "edited link matches new contract");
		}

		[TestMethod]
		public void Sync_Contracts_Removed()
		{
			_manager.PVs.Add(CreatePV(_pvContract));
			var newLinks = new PVContract[] { };

			var result = _manager.Sync(newLinks, CreatePV);

			Assert.IsNotNull(result, "result is not null");
			Assert.IsTrue(result.Changed, "is changed");
			Assert.AreEqual(0, result.Added.Length, "none added");
			Assert.AreEqual(0, result.Edited.Length, "none edited");
			Assert.AreEqual(1, result.Removed.Length, "1 removed");
			Assert.AreEqual(0, result.Unchanged.Length, "none unchanged");
			Assert.IsTrue(result.Removed.First().ContentEquals(_pvContract), "removed link matches contract");
		}
	}
}
