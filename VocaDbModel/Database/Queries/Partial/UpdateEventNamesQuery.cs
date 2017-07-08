using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Exceptions;

namespace VocaDb.Model.Database.Queries.Partial {

	public class UpdateEventNamesQuery {

		public void CheckDuplicateName(IDatabaseContext ctx, string[] names, int eventId) {

			var duplicateName = names
				.GroupBy(n => n, new KanaAndCaseInsensitiveStringComparer())
				.Where(n => n.Count() > 1)
				.Select(n => n.First())
				.FirstOrDefault();

			if (duplicateName != null) {
				throw new DuplicateEventNameException(duplicateName);
			}

			var duplicate = ctx.Query<EventName>()
				.FirstOrDefault(e => e.Entry.Id != eventId && names.Contains(e.Value));

			if (duplicate != null) {
				throw new DuplicateEventNameException(duplicate.Value);
			}

		}

		private bool SaveNames(IDatabaseContext ctx, ReleaseEvent ev, IEnumerable<ILocalizedString> names) {

			var diff = ev.Names.SyncByContent(names, ev,
				deleted => {
					foreach (var name in deleted)
						ctx.Delete(name);
					ctx.Flush();
				}
			);

			foreach (var n in diff.Added)
				ctx.Save(n);

			return diff.Changed;

		}

		private IEnumerable<ILocalizedString> GetNames(IDatabaseContext ctx,
			IEntryWithIntId seriesLink, bool customName, int seriesNumber, string seriesSuffix, IEnumerable<ILocalizedString> nameContracts) {

			var series = ctx.NullSafeLoad<ReleaseEventSeries>(seriesLink);

			var names = series != null && !customName
				? series.Names.Select(seriesName =>
					new LocalizedStringContract(series.GetEventName(seriesNumber, seriesSuffix, seriesName.Value), seriesName.Language))
				: nameContracts;

			return names;

		}

		public bool UpdateNames(IDatabaseContext ctx, ReleaseEvent ev, IEntryWithIntId seriesLink, 
			bool customName, int seriesNumber, string seriesSuffix, IEnumerable<ILocalizedString> nameContracts) {

			var names = GetNames(ctx, seriesLink, customName, seriesNumber, seriesSuffix, nameContracts).ToArray();
			var namesValues = names.Select(n => n.Value).ToArray();

			CheckDuplicateName(ctx, namesValues, ev.Id);
			var changed = SaveNames(ctx, ev, names);
			return changed;

		}

	}

}
