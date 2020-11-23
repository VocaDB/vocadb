using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Domain.Globalization
{

	/// <summary>
	/// Tests for <see cref="NameManager{T}"/>.
	/// </summary>
	[TestClass]
	public class NameManagerTests
	{

		class NameFactory : INameFactory<LocalizedStringWithId>
		{

			private readonly NameManager<LocalizedStringWithId> nameManager;

			public NameFactory(NameManager<LocalizedStringWithId> nameManager)
			{
				this.nameManager = nameManager;
			}

			public LocalizedStringWithId CreateName(string val, ContentLanguageSelection language)
			{
				var name = new LocalizedStringWithId(val, language);
				nameManager.Add(name);
				return name;
			}

		}

		private int id = 1;
		private NameFactory nameFactory;
		private NameManager<LocalizedStringWithId> nameManager;

		private LocalizedStringWithId AddName(string name, ContentLanguageSelection lang = ContentLanguageSelection.English)
		{
			var str = new LocalizedStringWithId(name, lang) { Id = id++ };
			nameManager.Add(str);
			return str;
		}

		private void AssertNames(CollectionDiff<LocalizedStringWithId, LocalizedStringWithId> result,
			IEnumerable<LocalizedStringWithIdContract> added = null,
			IEnumerable<LocalizedStringWithIdContract> removed = null, IEnumerable<LocalizedStringWithIdContract> unchanged = null)
		{

			AssertCollection(result.Added, added, "added");
			AssertCollection(result.Removed, removed, "removed");
			AssertCollection(result.Unchanged, unchanged, "unchanged");

		}

		private void AssertCollection(LocalizedStringWithId[] actual, IEnumerable<LocalizedStringWithIdContract> expected, string action)
		{

			Assert.AreEqual(expected?.Count() ?? 0, actual.Length, "Number of items " + action);

			if (expected == null)
				return;

			foreach (var item in actual)
			{
				Assert.IsTrue(actual.Any(n => n.ContentEquals(item)), string.Format("Found name '{0}' ({1})", item, action));
			}

		}

		private LocalizedStringWithIdContract Contract(string val, ContentLanguageSelection lang)
		{
			return new LocalizedStringWithIdContract { Value = val, Language = lang };
		}

		[TestInitialize]
		public void SetUp()
		{

			nameManager = new NameManager<LocalizedStringWithId>();
			nameFactory = new NameFactory(nameManager);

		}

		[TestMethod]
		public void GetAdditionalNamesStringForLanguage()
		{

			AddName("The Twins Sing to 1000 Digits of Pi", ContentLanguageSelection.English);
			AddName("あの双子が円周率1000桁に挑戦", ContentLanguageSelection.Japanese);
			AddName("Ano Futago ga Enshuuritsu 1000 Keta ni Chousen", ContentLanguageSelection.Romaji);

			var additionalNames = nameManager.GetAdditionalNamesStringForLanguage(ContentLanguagePreference.English);

			Assert.AreEqual("あの双子が円周率1000桁に挑戦, Ano Futago ga Enshuuritsu 1000 Keta ni Chousen", additionalNames, "Result");

		}

		[TestMethod]
		public void GetAdditionalNamesStringForLanguage_NoNames()
		{

			nameManager.UpdateSortNames();

			Assert.AreEqual(string.Empty, nameManager.GetAdditionalNamesStringForLanguage(ContentLanguagePreference.English), "Additional names string is empty");

		}

		[TestMethod]
		public void Sync_NewNames()
		{

			var added = new[] { Contract("Miku", ContentLanguageSelection.English) };

			var result = nameManager.Sync(added, nameFactory);

			AssertNames(result, added: added);
			Assert.AreEqual(1, nameManager.Names.Count, "Number of names");

		}

		[TestMethod]
		public void Sync_AddedAndRemoved()
		{

			AddName("Luka");

			var added = new[] { Contract("Miku", ContentLanguageSelection.English) };

			var result = nameManager.Sync(added, nameFactory);

			AssertCollection(result.Added, added, "added");
			Assert.AreEqual(1, result.Removed.Length, "Number of items removed");
			Assert.AreEqual(1, nameManager.Names.Count, "Number of names");

		}

		[TestMethod]
		public void Sync_Updated()
		{

			var name = new LocalizedStringWithIdContract(AddName("Miku"));
			var nameId = name.Id;
			name.Value = "Miku Miku";
			var updated = new[] { name };

			var result = nameManager.Sync(updated, nameFactory);

			AssertCollection(result.Edited, updated, "edited");
			Assert.AreEqual(1, nameManager.Names.Count, "Number of names");
			Assert.AreEqual("Miku Miku", nameManager.Names[0].Value, "Name was updated");
			Assert.AreEqual(nameId, nameManager.Names[0].Id, "Id remains unchanged");


		}

		[TestMethod]
		public void UpdateSortNames_NoNames()
		{

			nameManager.UpdateSortNames();

			Assert.AreEqual(string.Empty, nameManager.SortNames[ContentLanguagePreference.Japanese], "Japanese name is empty");

		}

		[TestMethod]
		public void UpdateSortNames_OnlyPrimaryName()
		{

			AddName("VocaliodP", ContentLanguageSelection.English);
			AddName("ぼーかりおどP", ContentLanguageSelection.Japanese);

			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.English], "Primary English name");
			Assert.AreEqual("ぼーかりおどP", nameManager.SortNames[ContentLanguageSelection.Japanese], "Primary Japanese name");
			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.Romaji], "Primary Romaji name");

		}

		[TestMethod]
		public void UpdateSortNames_DuplicateAliases()
		{

			AddName("VocaliodP", ContentLanguageSelection.English);
			AddName("noa", ContentLanguageSelection.Unspecified);
			AddName("noa", ContentLanguageSelection.Unspecified);

			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.English], "Primary English name");
			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.Japanese], "Primary Japanese name");
			Assert.AreEqual("noa", nameManager.AdditionalNamesString, "Additional names");

		}

	}
}
