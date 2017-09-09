
module vdb.viewModels.pvs {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;
	
	export class PVListEditViewModel {

		constructor(
			private readonly  repo: rep.PVRepository,
			public urlMapper: UrlMapper, // Used from the view to map to PV listing
			pvs: dc.pvs.PVContract[],
			public canBulkDeletePVs: boolean,
			public showPublishDates: boolean,
			public allowDisabled: boolean) {

			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);
			this.pvs = ko.observableArray(_.map(pvs, pv => new PVEditViewModel(pv)));

		}

		public add = () => {

			var newPvUrl = this.newPvUrl();

			if (!newPvUrl)
				return;

			var pvType = this.newPvType();

			this.repo.getPVByUrl(newPvUrl, this.newPvType(), pv => {
			
				this.newPvUrl("");
				this.isPossibleInstrumental(this.isPossibleInstrumentalPv(pv));
				this.pvs.push(new PVEditViewModel(pv, pvType));
					
			}).fail((jqXHR: JQueryXHR) => {

				if (jqXHR.statusText)
					alert(jqXHR.statusText);

			});

		}

		public formatLength = (seconds: number) => {
			return vdb.helpers.DateTimeHelper.formatFromSeconds(seconds);
		}

		public getPvServiceIcon = (service: string) => {
			return this.pvServiceIcons.getIconUrl(service);
		}

		public isPossibleInstrumental = ko.observable(false);

		// Attempts to identify whether the PV could be instrumental
		private isPossibleInstrumentalPv = (pv: dc.pvs.PVContract) => {

			return (pv && pv.name && (
				pv.name.toLowerCase().indexOf("inst.") >= 0
				|| pv.name.toLowerCase().indexOf("instrumental") >= 0
				|| pv.name.indexOf("カラオケ") >= 0
				|| pv.name.indexOf("オフボーカル") >= 0
			));

		}

		public newPvType = ko.observable("Original");

		public newPvUrl = ko.observable("");

		public pvs: KnockoutObservableArray<PVEditViewModel>;

		public pvServiceIcons: vdb.models.PVServiceIcons;

		public remove = (pv: PVEditViewModel) => {
			this.pvs.remove(pv);
		}

		public toContracts: () => dc.pvs.PVContract[] = () => {
			return ko.toJS(this.pvs());
		}

		public uploadMedia = () => {

			var input: any = $("#uploadMedia")[0];
			var fd = new FormData();

			fd.append('file', input.files[0]);
			$.ajax({
				url: "/Song/PostMedia/",
				data: fd,
				processData: false,
				contentType: false,
				type: 'POST',
				success: (result) => {
					this.pvs.push(new PVEditViewModel(result, 'Original'));
				},
				error: (result) => {
					var text = result.status === 404 ? "File too large" : result.statusText;
					alert("Unable to post file: " + text);
				}
			});

		}

	}

}