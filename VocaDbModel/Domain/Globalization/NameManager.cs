using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Helpers;
using System.Collections;

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

		/// <summary>
		/// List of all name values.
		/// This list includes translated sort name as well as aliases, 
		/// meaning union of <see cref="SortNames"/> and <see cref="Names"/>.
		/// Duplicates are excluded.
		/// Cannot be null.
		/// This list is not persisted to the database.
		/// </summary>
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

		public T FirstName(ContentLanguageSelection languageSelection) {
			return Names.FirstOrDefault(n => n.Language == languageSelection);
		}

		public LocalizedStringWithId FirstNameBase(ContentLanguageSelection languageSelection) {
			return FirstName(languageSelection);
		}

		/// <summary>
		/// Gets a comma-separated list of additional names for a specific language.
		/// 
		/// For example, if the entry has names A, B and C for Japanese, Romaji and English respectively,
		/// the return value for English would be "A, B", and for Japanese it would be "B, C".
		/// </summary>
		/// <param name="languagePreference">Language preference indicating the primary display name (which is EXCLUDED from this string).</param>
		/// <returns>Additional names string. Cannot be null.</returns>
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

		/// <summary>
		/// Gets entry name (with both primary display name and list of additional names)
		/// for a specific language.
		/// </summary>
		/// <param name="languagePreference">Language preference.</param>
		/// <returns>Entry name. Cannot be null.</returns>
		public EntryNameContract GetEntryName(ContentLanguagePreference languagePreference) {

			var display = SortNames[languagePreference];
			var additional = GetAdditionalNamesStringForLanguage(languagePreference);

			return new EntryNameContract(display, additional);

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

		public virtual CollectionDiffWithValue<T,T> Sync(IEnumerable<LocalizedStringWithIdContract> newNames, INameFactory<T> nameFactory) {

			ParamIs.NotNull(() => newNames);
			ParamIs.NotNull(() => nameFactory);

			var diff = CollectionHelper.Diff(Names, newNames, (n1, n2) => n1.Id == n2.Id);
			var created = new List<T>();
			var edited = new List<T>();

			foreach (var n in diff.Removed) {
				Remove(n);
			}

			foreach (var nameEntry in newNames) {

				var entry = nameEntry;
				var old = (entry.Id != 0 ? Names.FirstOrDefault(n => n.Id == entry.Id) : null);

				if (old != null) {

					if (!old.ContentEquals(nameEntry)) {
						old.Language = nameEntry.Language;
						old.Value = nameEntry.Value;
						edited.Add(old);
					}

				} else {

					var n = nameFactory.CreateName(nameEntry.Value, nameEntry.Language);
					created.Add(n);

				}

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
}
