using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Tests.Domain.Globalization {

	/// <summary>
	/// Tests for <see cref="NameManager{T}"/>.
	/// </summary>
	[TestClass]
	public class NameManagerTests {

		private NameManager<LocalizedStringWithId> nameManager;

		private void AddName(string name, ContentLanguageSelection lang) {
			nameManager.Add(new LocalizedStringWithId(name, lang));
		}

		[TestInitialize]
		public void SetUp() {

			nameManager = new NameManager<LocalizedStringWithId>();

		}

		[TestMethod]
		public void GetAdditionalNamesStringForLanguage() {
			
			AddName("The Twins Sing to 1000 Digits of Pi", ContentLanguageSelection.English);
			AddName("あの双子が円周率1000桁に挑戦", ContentLanguageSelection.Japanese);
			AddName("Ano Futago ga Enshuuritsu 1000 Keta ni Chousen", ContentLanguageSelection.Romaji);

			var additionalNames = nameManager.GetAdditionalNamesStringForLanguage(ContentLanguagePreference.English);

			Assert.AreEqual("あの双子が円周率1000桁に挑戦, Ano Futago ga Enshuuritsu 1000 Keta ni Chousen", additionalNames, "Result");

		}

		[TestMethod]
		public void GetAdditionalNamesStringForLanguage_NoNames() {
			
			nameManager.UpdateSortNames();

			Assert.AreEqual(string.Empty, nameManager.GetAdditionalNamesStringForLanguage(ContentLanguagePreference.English), "Additional names string is empty");

		}

		[TestMethod]
		public void UpdateSortNames_NoNames() {

			nameManager.UpdateSortNames();

			Assert.AreEqual(string.Empty, nameManager.SortNames[ContentLanguagePreference.Japanese], "Japanese name is empty");

		}

		[TestMethod]
		public void UpdateSortNames_OnlyPrimaryName() {

			AddName("VocaliodP", ContentLanguageSelection.English);
			AddName("ぼーかりおどP", ContentLanguageSelection.Japanese);

			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.English], "Primary English name");
			Assert.AreEqual("ぼーかりおどP", nameManager.SortNames[ContentLanguageSelection.Japanese], "Primary Japanese name");
			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.Romaji], "Primary Romaji name");

		}

		[TestMethod]
		public void UpdateSortNames_DuplicateAliases() {

			AddName("VocaliodP", ContentLanguageSelection.English);
			AddName("noa", ContentLanguageSelection.Unspecified);
			AddName("noa", ContentLanguageSelection.Unspecified);

			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.English], "Primary English name");
			Assert.AreEqual("VocaliodP", nameManager.SortNames[ContentLanguageSelection.Japanese], "Primary Japanese name");
			Assert.AreEqual("noa", nameManager.AdditionalNamesString, "Additional names");

		}

	}
}
