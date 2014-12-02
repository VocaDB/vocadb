using System;
using System.ComponentModel.DataAnnotations;
using VocaDb.Model;
using VocaDb.Model.DataContracts.ReleaseEvents;
using System.Web.Mvc;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Event {

	public class EventEdit {

		public EventEdit() {
			Description = SeriesSuffix = string.Empty;
		}

		public EventEdit(ReleaseEventSeriesContract seriesContract)
			: this() {

			CopyNonEditableProperties(seriesContract);

		}

		public EventEdit(ReleaseEventDetailsContract contract)
			: this() {

			ParamIs.NotNull(() => contract);

			Date = contract.Date;
			Description = contract.Description;
			Id = contract.Id;
			Name = OldName = contract.Name;
			SeriesNumber = contract.SeriesNumber;
			SeriesSuffix = contract.SeriesSuffix;

			CopyNonEditableProperties(contract);

		}

		public ReleaseEventSeriesContract[] AllSeries { get; set; }

		[CultureInvariantDateTimeModelBinder("Date")]
		public DateTime? Date { get; set; }

		[StringLength(400)]
		public string Description { get; set; }

		public int Id { get; set; }

		[StringLength(50)]
		public string Name { get; set; }

		public string OldName { get; set; }

		public int? SeriesId { get; set; }

		public string SeriesName { get; set; }

		[Display(Name = "Series suffix")]
		public string SeriesSuffix { get; set; }

		[Display(Name = "Series number")]
		public int SeriesNumber { get; set; }

		public void CopyNonEditableProperties(ReleaseEventDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			AllSeries = contract.AllSeries;
			OldName = contract.Name;

			CopyNonEditableProperties(contract.Series);

		}

		public void CopyNonEditableProperties(ReleaseEventSeriesContract seriesContract) {

			if (seriesContract != null) {
				SeriesId = seriesContract.Id;
				SeriesName = seriesContract.Name;
			}

		}

		public ReleaseEventDetailsContract ToContract() {

			return new ReleaseEventDetailsContract {
				Date = this.Date,
				Description = this.Description ?? string.Empty,
				Id = this.Id,
				Name = this.Name,
				Series = (this.SeriesId != null ? new ReleaseEventSeriesContract { Id = this.SeriesId.Value, Name = this.SeriesName } : null), 
				SeriesNumber = this.SeriesNumber,
				SeriesSuffix = this.SeriesSuffix ?? string.Empty
			};

		}

	}

}