using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Helpers;
using System.Collections;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain.Globalization {

	public class NameManager<T> : INameManager<T>, IEnumerable<T> where T : LocalizedStringWithId {

		private string additionalNamesString;
		private IList<T> names = new List<T>();
		private TranslatedString sortNames = new TranslatedString();

		private T GetDefaultName() {

			if (!Names.Any())
				return null;

			var name = FirstName(sortNames.DefaultLanguage);
			
			return name ?? Names.First();

		}

		private T GetFirstName(ContentLanguageSelection languageSelection) {

			if (!Names.Any())
				return null;

			var name = FirstName(languageSelection);

			// Substitute English with Romaji
			if (name == null && languageSelection == ContentLanguageSelection.English)
				name = FirstName(ContentLanguageSelection.Romaji);

			// Substitute Romaji with English
			if (name == null && languageSelection == ContentLanguageSelection.Romaji)
				name = FirstName(ContentLanguageSelection.English);

			return name ?? GetDefaultName();

		}

		private void SetValueFor(ContentLanguageSelection language) {

			if (!Names.Any())
				return;

			var name = GetFirstName(language);

			if (name != null)
				SortNames[language] = name.Value;

			if (string.IsNullOrEmpty(SortNames[language]))
				SortNames[language] = Names.First().Value;

		}

		/// <summary>
		/// Comma-separated string containing names that aren't part of any sort name.
		/// This can be used to construct the additional names string without loading the full list of names from the DB.
		/// </summary>
		public virtual string AdditionalNamesString {
			get { return additionalNamesString; }
			set {
				ParamIs.NotNull(() => value);
				additionalNamesString = value; 
			}
		}

		public virtual IEnumerable<string> AllValues {
			get {

				return SortNames.All
					.Concat(Names.Select(n => n.Value))
					.Distinct();

			}
		}

		/// <summary>
		/// List of names.
		/// This is the list of assigned names for a entry.
		/// 
		/// This list does not automatically include the sort names:
		/// the entry might have a translated sort name (<see cref="SortNames"/>)
		/// even though this list is empty.
		/// 
		/// The sort names are generated from this list.
		/// This list is persisted to the database.
		/// 
		/// Cannot be null.
		/// </summary>
		public virtual IList<T> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		public virtual IEnumerable<LocalizedStringWithId> NamesBase {
			get { return Names; }
		}

		public virtual TranslatedString SortNames {
			get { return sortNames; }
			set {
				ParamIs.NotNull(() => value);
				sortNames = value;
			}
		}

		public virtual void Add(T name, bool update = true) {
			
			Names.Add(name);

			if (update)
				UpdateSortNames();

		}

		/// <summary>
		/// Gets the first name matching a language selection.
		/// Language substitutions are *not* applied.
		/// </summary>
		/// <param name="languageSelection">Language selection.</param>
		/// <returns>Name. Can be null if there is no name for the specfied language selection.</returns>
		public T FirstName(ContentLanguageSelection languageSelection) {
			return Names.FirstOrDefault(n => n.Language == languageSelection);
		}

		public LocalizedStringWithId FirstNameBase(ContentLanguageSelection languageSelection) {
			return FirstName(languageSelection);
		}

		public string FirstNameValue(ContentLanguageSelection languageSelection) {
			var name = FirstName(languageSelection);
			return name != null ? name.Value : null;
		}

		public string GetAdditionalNamesStringForLanguage(ContentLanguagePreference languagePreference) {

			var display = SortNames[languagePreference];
			var different = SortNames.All.Where(s => s != display).Distinct();

			if (!string.IsNullOrEmpty(AdditionalNamesString))
				return string.Join(", ", different.Concat(Enumerable.Repeat(AdditionalNamesString, 1)));
			else
				return string.Join(", ", different);

		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Names.GetEnumerator();
		}

		public virtual IEnumerator<T> GetEnumerator() {
			return Names.GetEnumerator();
		}

		public EntryNameContract GetEntryName(ContentLanguagePreference languagePreference) {

			var display = SortNames[languagePreference];
			var additional = GetAdditionalNamesStringForLanguage(languagePreference);

			return new EntryNameContract(display, additional);

		}

		public virtual string GetUrlFriendlyName() {
			return UrlFriendlyNameFactory.GetUrlFriendlyName(this);
		}

		public virtual bool HasName(LocalizedString name) {

			return Names.Any(n => n.ContentEquals(name));

		}

		public virtual bool HasNameForLanguage(ContentLanguageSelection language) {
			return FirstName(language) != null;
		}

		public virtual bool HasName(string val) {

			return Names.Any(n => n.Value.Equals(val, StringComparison.InvariantCultureIgnoreCase));

		}

		public virtual void Init(IEnumerable<LocalizedStringContract> names, INameFactory<T> nameFactory) {

			ParamIs.NotNull(() => names);
			ParamIs.NotNull(() => nameFactory);

			foreach (var name in names)
				nameFactory.CreateName(name.Value, name.Language);

			if (names.Any(n => n.Language == ContentLanguageSelection.Japanese))
				SortNames.DefaultLanguage = ContentLanguageSelection.Japanese;
			else if (names.Any(n => n.Language == ContentLanguageSelection.Romaji))
				SortNames.DefaultLanguage = ContentLanguageSelection.Romaji;
			else if (names.Any(n => n.Language == ContentLanguageSelection.English))
				SortNames.DefaultLanguage = ContentLanguageSelection.English;

		}

		public virtual void Remove(T name, bool update = true) {

			Names.Remove(name);

			if (update)
				UpdateSortNames();

		}

		public virtual CollectionDiffWithValue<T,T> Sync(IEnumerable<LocalizedStringWithIdContract> newNames, INameFactory<T> nameFactory,
			Action<T[]> deletedCallback = null, Action<T[]> editedCallback = null) {

			ParamIs.NotNull(() => newNames);
			ParamIs.NotNull(() => nameFactory);

			var diff = CollectionHelper.Diff(Names, newNames, (n1, n2) => n1.Id == n2.Id);
			var created = new List<T>();
			var edited = new List<T>();

			foreach (var n in diff.Removed) {
				Remove(n);
			}

			if (deletedCallback != null)
				deletedCallback(diff.Removed);

			foreach (var old in diff.Unchanged) {

				var nameEntry = newNames.First(n => n.Id == old.Id);

				if (!old.ContentEquals(nameEntry)) {
					old.Language = nameEntry.Language;
					old.Value = nameEntry.Value;
					edited.Add(old);
				}

			}

			if (editedCallback != null)
				editedCallback(edited.ToArray());

			foreach (var nameEntry in diff.Added) {

				var n = nameFactory.CreateName(nameEntry.Value, nameEntry.Language);
				created.Add(n);

			}

			UpdateSortNames();

			return new CollectionDiffWithValue<T,T>(created, diff.Removed, diff.Unchanged, edited);

		}

		public virtual CollectionDiff<T, T> SyncByContent(IEnumerable<LocalizedStringContract> newNames, INameFactory<T> nameFactory) {

			ParamIs.NotNull(() => newNames);
			ParamIs.NotNull(() => nameFactory);

			var diff = CollectionHelper.Diff(Names, newNames, (n1, n2) => n1.ContentEquals(n2));
			var created = new List<T>();

			foreach (var n in diff.Removed) {
				Remove(n);
			}

			foreach (var nameEntry in diff.Added) {

				var n = nameFactory.CreateName(nameEntry.Value, nameEntry.Language);
				created.Add(n);

			}

			UpdateSortNames();

			return new CollectionDiff<T, T>(created, diff.Removed, diff.Unchanged);

		}

		public virtual void UpdateSortNames() {

			if (!Names.Any())
				return;

			var languages = new[] { ContentLanguageSelection.Japanese, ContentLanguageSelection.Romaji, ContentLanguageSelection.English };

			foreach (var l in languages)
				SetValueFor(l);

			var additionalNames = Names.Select(n => n.Value).Where(n => !SortNames.All.Contains(n)).Distinct();
			AdditionalNamesString = string.Join(", ", additionalNames);

		}

	}

	public class BasicNameManager : NameManager<LocalizedStringWithId> {

		public BasicNameManager() { }

		public BasicNameManager(INameManager nameManager) {
			
			ParamIs.NotNull(() => nameManager);

			Names = nameManager.NamesBase.Select(n => new LocalizedStringWithId(n.Value, n.Language)).ToArray();
			SortNames = new TranslatedString(nameManager.SortNames);

		}

	}

}
