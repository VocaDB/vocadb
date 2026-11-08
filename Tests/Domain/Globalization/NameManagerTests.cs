#nullable disable

using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Domain.Globalization;

/// <summary>
/// Tests for <see cref="NameManager{T}"/>.
/// </summary>
[TestClass]
public class NameManagerTests
{
	class NameFactory : INameFactory<LocalizedStringWithId>
	{
		private readonly NameManager<LocalizedStringWithId> _nameManager;

		public NameFactory(NameManager<LocalizedStringWithId> nameManager)
		{
			_nameManager = nameManager;
		}

		public LocalizedStringWithId CreateName(string val, ContentLanguageSelection language)
		{
			var name = new LocalizedStringWithId(val, language);
			_nameManager.Add(name);
			return name;
		}
	}

	private int _id = 1;
	private NameFactory _nameFactory;
	private NameManager<LocalizedStringWithId> _nameManager;

	private LocalizedStringWithId AddName(string name, ContentLanguageSelection lang = ContentLanguageSelection.English)
	{
		var str = new LocalizedStringWithId(name, lang) { Id = _id++ };
		_nameManager.Add(str);
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
		actual.Length.Should().Be(expected?.Count() ?? 0, "Number of items " + action);

		if (expected == null)
			return;

		foreach (var item in actual)
		{
			actual.Any(n => n.ContentEquals(item)).Should().BeTrue($"Found name '{item}' ({action})");
		}
	}

	private LocalizedStringWithIdContract Contract(string val, ContentLanguageSelection lang)
	{
		return new LocalizedStringWithIdContract { Value = val, Language = lang };
	}

	[TestInitialize]
	public void SetUp()
	{
		_nameManager = new NameManager<LocalizedStringWithId>();
		_nameFactory = new NameFactory(_nameManager);
	}

	[TestMethod]
	public void GetAdditionalNamesStringForLanguage()
	{
		AddName("The Twins Sing to 1000 Digits of Pi", ContentLanguageSelection.English);
		AddName("あの双子が円周率1000桁に挑戦", ContentLanguageSelection.Japanese);
		AddName("Ano Futago ga Enshuuritsu 1000 Keta ni Chousen", ContentLanguageSelection.Romaji);

		var additionalNames = _nameManager.GetAdditionalNamesStringForLanguage(ContentLanguagePreference.English);

		additionalNames.Should().Be("あの双子が円周率1000桁に挑戦, Ano Futago ga Enshuuritsu 1000 Keta ni Chousen", "Result");
	}

	[TestMethod]
	public void GetAdditionalNamesStringForLanguage_NoNames()
	{
		_nameManager.UpdateSortNames();

		_nameManager.GetAdditionalNamesStringForLanguage(ContentLanguagePreference.English).Should().Be(string.Empty, "Additional names string is empty");
	}

	[TestMethod]
	public void Sync_NewNames()
	{
		var added = new[] { Contract("Miku", ContentLanguageSelection.English) };

		var result = _nameManager.Sync(added, _nameFactory);

		AssertNames(result, added: added);
		_nameManager.Names.Count.Should().Be(1, "Number of names");
	}

	[TestMethod]
	public void Sync_AddedAndRemoved()
	{
		AddName("Luka");

		var added = new[] { Contract("Miku", ContentLanguageSelection.English) };

		var result = _nameManager.Sync(added, _nameFactory);

		AssertCollection(result.Added, added, "added");
		result.Removed.Length.Should().Be(1, "Number of items removed");
		_nameManager.Names.Count.Should().Be(1, "Number of names");
	}

	[TestMethod]
	public void Sync_Updated()
	{
		var name = new LocalizedStringWithIdContract(AddName("Miku"));
		var nameId = name.Id;
		name.Value = "Miku Miku";
		var updated = new[] { name };

		var result = _nameManager.Sync(updated, _nameFactory);

		AssertCollection(result.Edited, updated, "edited");
		_nameManager.Names.Count.Should().Be(1, "Number of names");
		_nameManager.Names[0].Value.Should().Be("Miku Miku", "Name was updated");
		_nameManager.Names[0].Id.Should().Be(nameId, "Id remains unchanged");
	}

	[TestMethod]
	public void Sync_Updated_CleansNameAndSearchValue()
	{
		var existing = AddName("Miku");
		var contract = new LocalizedStringWithIdContract(existing)
		{
			Value = "Miku\u0008Miku",
			Language = ContentLanguageSelection.Japanese,
		};

		var result = _nameManager.Sync(new[] { contract }, _nameFactory);

		result.Edited.Should().ContainSingle().Which.Should().BeSameAs(existing);
		existing.Value.Should().Be("MikuMiku");
		existing.SearchValue.Should().Be("MIKUMIKU");
		existing.Id.Should().Be(contract.Id);
		existing.Language.Should().Be(ContentLanguageSelection.Japanese);
		contract.Value.Should().Be("Miku\u0008Miku");
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Sync_CleanedNameIsUnchanged(bool immutable)
	{
		var existing = AddName("MikuMiku");
		var contract = new LocalizedStringWithIdContract(existing) { Value = "Miku\u0008Miku" };

		var result = _nameManager.Sync(new[] { contract }, _nameFactory, immutable: immutable);

		result.Changed.Should().BeFalse();
		_nameManager.Names.Should().ContainSingle().Which.Should().BeSameAs(existing);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Sync_ControlOnlyNameRejectedBeforeChanges(bool updateExisting)
	{
		var existing = AddName("Miku");
		var contract = new LocalizedStringWithIdContract
		{
			Id = updateExisting ? existing.Id : 0,
			Value = "\u0008",
			Language = ContentLanguageSelection.English,
		};

		Invoking(() => _nameManager.Sync(new[] { contract }, _nameFactory)).Should().Throw<ArgumentException>();

		_nameManager.Names.Should().ContainSingle().Which.Should().BeSameAs(existing);
		existing.Value.Should().Be("Miku");
	}

	[TestMethod]
	public void SyncByContent_CleanedNameIsUnchanged()
	{
		var existing = AddName("MikuMiku");
		var contract = Contract("Miku\u0008Miku", ContentLanguageSelection.English);

		var result = _nameManager.SyncByContent(new[] { contract }, _nameFactory);

		result.Changed.Should().BeFalse();
		_nameManager.Names.Should().ContainSingle().Which.Should().BeSameAs(existing);
	}

	[TestMethod]
	public void SyncByContent_ControlOnlyNameRejectedBeforeChanges()
	{
		var existing = AddName("Miku");
		var contract = Contract("\u0008", ContentLanguageSelection.English);

		Invoking(() => _nameManager.SyncByContent(new[] { contract }, _nameFactory)).Should().Throw<ArgumentException>();

		_nameManager.Names.Should().ContainSingle().Which.Should().BeSameAs(existing);
	}

	[TestMethod]
	public void Init_ControlOnlyNameRejectedBeforeChanges()
	{
		var contracts = new[]
		{
			Contract("Miku", ContentLanguageSelection.English),
			Contract("\u0008", ContentLanguageSelection.Japanese),
		};

		Invoking(() => _nameManager.Init(contracts, _nameFactory)).Should().Throw<ArgumentException>();

		_nameManager.Names.Should().BeEmpty();
	}

	[TestMethod]
	public void Sync_KeepsWhitespaceName()
	{
		var contract = Contract("\t\n\r", ContentLanguageSelection.English);

		_nameManager.Sync(new[] { contract }, _nameFactory);

		_nameManager.Names.Should().ContainSingle().Which.Value.Should().Be("\t\n\r");
	}

	[TestMethod]
	public void UpdateSortNames_NoNames()
	{
		_nameManager.UpdateSortNames();

		_nameManager.SortNames[ContentLanguagePreference.Japanese].Should().Be(string.Empty, "Japanese name is empty");
	}

	[TestMethod]
	public void UpdateSortNames_OnlyPrimaryName()
	{
		AddName("VocaliodP", ContentLanguageSelection.English);
		AddName("ぼーかりおどP", ContentLanguageSelection.Japanese);

		_nameManager.SortNames[ContentLanguageSelection.English].Should().Be("VocaliodP", "Primary English name");
		_nameManager.SortNames[ContentLanguageSelection.Japanese].Should().Be("ぼーかりおどP", "Primary Japanese name");
		_nameManager.SortNames[ContentLanguageSelection.Romaji].Should().Be("VocaliodP", "Primary Romaji name");
	}

	[TestMethod]
	public void UpdateSortNames_DuplicateAliases()
	{
		AddName("VocaliodP", ContentLanguageSelection.English);
		AddName("noa", ContentLanguageSelection.Unspecified);
		AddName("noa", ContentLanguageSelection.Unspecified);

		_nameManager.SortNames[ContentLanguageSelection.English].Should().Be("VocaliodP", "Primary English name");
		_nameManager.SortNames[ContentLanguageSelection.Japanese].Should().Be("VocaliodP", "Primary Japanese name");
		_nameManager.AdditionalNamesString.Should().Be("noa", "Additional names");
	}
}
