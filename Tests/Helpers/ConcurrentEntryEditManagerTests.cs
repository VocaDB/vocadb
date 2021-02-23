#nullable disable

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Security;

namespace VocaDb.Tests.Helpers
{
	/// <summary>
	/// Unit tests for <see cref="ConcurrentEntryEditManager"/>.
	/// </summary>
	[TestClass]
	public class ConcurrentEntryEditManagerTests
	{
		private EntryRef _entryRef;
		private ConcurrentEntryEditManager _manager;
		private User _miku;

		[TestInitialize]
		public void SetUp()
		{
			_entryRef = new EntryRef(EntryType.Artist, 39);
			_manager = new ConcurrentEntryEditManager();
			_miku = new User("Miku", "3939", "miku@vocadb.net", PasswordHashAlgorithms.Default) { Id = 1 };
		}

		/// <summary>
		/// No one is editing.
		/// </summary>
		[TestMethod]
		public void CheckConcurrentEdits_NoOneEditing()
		{
			var result = _manager.CheckConcurrentEditsInst(_entryRef, _miku);

			result.UserId.Should().Be(ConcurrentEntryEditManager.Nothing.UserId, "no one editing");
		}

		/// <summary>
		/// Another editor has just started editing.
		/// </summary>
		[TestMethod]
		public void CheckConcurrentEdits_PreviousEditor()
		{
			var rin = new User("Rin", "222", "rin@vocadb.net", PasswordHashAlgorithms.Default) { Id = 2 };
			_manager.CheckConcurrentEditsInst(_entryRef, rin);
			var result = _manager.CheckConcurrentEditsInst(_entryRef, _miku);

			result.UserId.Should().Be(rin.Id, "Rin still editing");
		}

		/// <summary>
		/// Another editor has edited the entry, but the expiration time has passed.
		/// </summary>
		[TestMethod]
		public void CheckConcurrentEdits_PreviousEditorExpired()
		{
			var rin = new User("Rin", "222", "rin@vocadb.net", PasswordHashAlgorithms.Default) { Id = 2 };
			_manager.CheckConcurrentEditsInst(_entryRef, rin);
			var result = _manager.CheckConcurrentEditsInst(_entryRef, _miku);
			result.Time = DateTime.MinValue;
			result = _manager.CheckConcurrentEditsInst(_entryRef, _miku);

			result.UserId.Should().Be(ConcurrentEntryEditManager.Nothing.UserId, "no one editing");
		}
	}
}
