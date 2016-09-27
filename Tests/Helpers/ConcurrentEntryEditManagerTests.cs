using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

    /// <summary>
    /// Unit tests for <see cref="ConcurrentEntryEditManager"/>.
    /// </summary>
    [TestClass]
    public class ConcurrentEntryEditManagerTests {

        private EntryRef entryRef;
		private ConcurrentEntryEditManager manager;
		private User miku;

        [TestInitialize]
        public void SetUp() {
            
            entryRef = new EntryRef(EntryType.Artist, 39);
			manager = new ConcurrentEntryEditManager();
            miku = new User("Miku", "3939", "miku@vocadb.net", "39") { Id = 1 };

        }

		/// <summary>
		/// No one is editing.
		/// </summary>
        [TestMethod]
        public void CheckConcurrentEdits_NoOneEditing() {

			var result = manager.CheckConcurrentEditsInst(entryRef, miku);

            Assert.AreEqual(ConcurrentEntryEditManager.Nothing.UserId, result.UserId, "no one editing");

        }

		/// <summary>
		/// Another editor has just started editing.
		/// </summary>
		[TestMethod]
		public void CheckConcurrentEdits_PreviousEditor() {

			var rin = new User("Rin", "222", "rin@vocadb.net", "2") { Id = 2 };
			manager.CheckConcurrentEditsInst(entryRef, rin);
			var result = manager.CheckConcurrentEditsInst(entryRef, miku);

			Assert.AreEqual(rin.Id, result.UserId, "Rin still editing");

		}

		/// <summary>
		/// Another editor has edited the entry, but the expiration time has passed.
		/// </summary>
		[TestMethod]
		public void CheckConcurrentEdits_PreviousEditorExpired() {

			var rin = new User("Rin", "222", "rin@vocadb.net", "2") { Id = 2 };
			manager.CheckConcurrentEditsInst(entryRef, rin);
			var result = manager.CheckConcurrentEditsInst(entryRef, miku);
			result.Time = DateTime.MinValue;
			result = manager.CheckConcurrentEditsInst(entryRef, miku);

			Assert.AreEqual(ConcurrentEntryEditManager.Nothing.UserId, result.UserId, "no one editing");

		}

    }

}
