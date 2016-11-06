
namespace vdb.viewModels.admin {

	import dc = dataContracts;

	export class ManageTagMappingsViewModel {

		constructor(private urlMapper: vdb.UrlMapper) {
			this.loadMappings();
		}

		public addMapping = () => {

			if (!this.newSourceName || this.newTargetTag.isEmpty())
				return;

			if (_.some(this.mappings(), m => m.sourceTag.toLowerCase() === this.newSourceName().toLowerCase())) {
				ui.showErrorMessage("Mapping already exists for source tag " + this.newSourceName());
				return;
			}

			this.mappings.push({ tag: this.newTargetTag.entry(), sourceTag: this.newSourceName(), isNew: true });
			this.newSourceName("");
			this.newTargetTag.clear();

		}

		public getSourceTagUrl = (tag: dc.tags.TagMappingContract) => {
			return "http://www.nicovideo.jp/tag/" + encodeURIComponent(tag.sourceTag);
		}

		public getTagUrl = (tag: dc.tags.TagMappingContract) => {
			return vdb.functions.mapFullUrl(utils.EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug));
		}

		private loadMappings = () => {

			$.getJSON(this.urlMapper.mapRelative("/api/tags/mappings"), result => {
				this.mappings(result);
			});

		}

		public mappings = ko.observableArray<dc.tags.TagMappingContract>();

		public newSourceName = ko.observable("");
		public newTargetTag = new BasicEntryLinkViewModel<dc.TagBaseContract>();

		public save = () => {

			var url = this.urlMapper.mapRelative("/api/tags/mappings");
			helpers.AjaxHelper.putJSON(url, this.mappings(), () => {
				ui.showSuccessMessage("Saved");
			});

		}

		public sortedMappings = ko.computed(() => _.sortBy(this.mappings(), m => m.tag.name.toLowerCase()));

	}

}