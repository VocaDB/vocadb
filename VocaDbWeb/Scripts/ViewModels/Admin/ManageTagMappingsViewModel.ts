
namespace vdb.viewModels.admin {

	import dc = dataContracts;

	export class ManageTagMappingsViewModel {

		constructor(private urlMapper: vdb.UrlMapper) {
			this.loadMappings();
		}

		public addMapping = () => {

			if (!this.newSourceName || this.newTargetTag.isEmpty())
				return;

			if (_.some(this.mappings(), m => m.tag.id === this.newTargetTag.id() && m.sourceTag.toLowerCase() === this.newSourceName().toLowerCase())) {
				ui.showErrorMessage("Mapping already exists for source tag " + this.newSourceName());
				return;
			}

			this.mappings.push(new EditTagMappingViewModel({ tag: this.newTargetTag.entry(), sourceTag: this.newSourceName() }, true));
			this.newSourceName("");
			this.newTargetTag.clear();

		}

		public deleteMapping = (mapping: EditTagMappingViewModel) => {
			mapping.isDeleted(true);
		}

		public getSourceTagUrl = (tag: EditTagMappingViewModel) => {
			return "http://www.nicovideo.jp/tag/" + encodeURIComponent(tag.sourceTag);
		}

		public getTagUrl = (tag: EditTagMappingViewModel) => {
			return vdb.functions.mapFullUrl(utils.EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug));
		}

		private loadMappings = () => {

			$.getJSON(this.urlMapper.mapRelative("/api/tags/mappings"), (result: dc.tags.TagMappingContract[]) => {
				this.mappings(_.map(result, t => new EditTagMappingViewModel(t)));
			});

		}

		public mappings = ko.observableArray<EditTagMappingViewModel>();

		public activeMappings = ko.computed(() => _.filter(this.mappings(), m => !m.isDeleted()));

		public newSourceName = ko.observable("");
		public newTargetTag = new BasicEntryLinkViewModel<dc.TagBaseContract>();

		public save = () => {

			var url = this.urlMapper.mapRelative("/api/tags/mappings");
			var mappings = ko.toJS(this.activeMappings());
			helpers.AjaxHelper.putJSON(url, mappings, () => {
				ui.showSuccessMessage("Saved");
				this.loadMappings();
			});

		}

		public sortedMappings = ko.computed(() => _.sortBy(this.mappings(), m => m.tag.name.toLowerCase()));

	}

	export class EditTagMappingViewModel {

		constructor(mapping: dc.tags.TagMappingContract, isNew: boolean = false) {
			this.sourceTag = mapping.sourceTag;
			this.tag = mapping.tag;
			this.isNew = isNew;
		}

		isDeleted = ko.observable(false);
		isNew: boolean;
		sourceTag: string;
		tag: dc.TagBaseContract;

		public deleteMapping = () => this.isDeleted(true);

	}

}