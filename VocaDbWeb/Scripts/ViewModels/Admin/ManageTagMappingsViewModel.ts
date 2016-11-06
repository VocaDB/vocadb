
namespace vdb.viewModels.admin {

	import dc = dataContracts;

	export class ManageTagMappingsViewModel {

		constructor(private urlMapper: vdb.UrlMapper) {

			$.getJSON(urlMapper.mapRelative("/api/tags/mappings"), result => {
				this.mappings(_.sortBy(result, (r: dc.tags.TagMappingContract) => r.tag.name.toLowerCase()));
			});

		}

		public addMapping = () => {

			if (!this.newSourceName || this.newTargetTag.isEmpty())
				return;

			this.mappings.push({ tag: this.newTargetTag.entry(), sourceTag: this.newSourceName() });
			this.newSourceName("");
			this.newTargetTag.clear();

		}

		public getTagUrl = (tag: dc.tags.TagMappingContract) => {
			return vdb.utils.EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug);
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

	}

}