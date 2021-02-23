#nullable disable

using System.Linq;
using FluentAssertions;
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
			_pvContract.ContentEquals(_pvContract2).Should().BeFalse("PVContracts are not equal");
		}

		[TestMethod]
		public void Sync_Contracts_NoExistingLinks()
		{
			var newPVs = new[] { _pvContract };

			var result = _manager.Sync(newPVs, CreatePV);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(1, "1 added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(0, "none unchanged");
			result.Added.First().ContentEquals(_pvContract).Should().BeTrue("added PV matches contract");
		}

		[TestMethod]
		public void Sync_Contracts_NotChanged()
		{
			_manager.PVs.Add(CreatePV(_pvContract));
			var newLinks = new[] { _pvContract };

			var result = _manager.Sync(newLinks, CreatePV);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeFalse("is not changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(1, "1 unchanged");
			result.Unchanged.First().ContentEquals(_pvContract).Should().BeTrue("unchanged PV matches contract");
		}

		[TestMethod]
		public void Sync_Contracts_Edited()
		{
			var oldPV = CreatePV(_pvContract);
			oldPV.Id = 2;
			_manager.PVs.Add(oldPV);
			var newLinks = new[] { _pvContract2 };

			var result = _manager.Sync(newLinks, CreatePV);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(1, "1 edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(1, "1 unchanged");
			result.Edited.First().ContentEquals(_pvContract2).Should().BeTrue("edited link matches new contract");
		}

		[TestMethod]
		public void Sync_Contracts_Removed()
		{
			_manager.PVs.Add(CreatePV(_pvContract));
			var newLinks = new PVContract[] { };

			var result = _manager.Sync(newLinks, CreatePV);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(1, "1 removed");
			result.Unchanged.Length.Should().Be(0, "none unchanged");
			result.Removed.First().ContentEquals(_pvContract).Should().BeTrue("removed link matches contract");
		}
	}
}
