/// <reference path="../typings/knockout/knockout.d.ts" />

namespace vdb.viewModels.tags {

	import dc = vdb.dataContracts;

	export class TagEditViewModel {

		// Bitmask for all possible entry types (all bits 1)
		public static readonly allEntryTypes = 1073741823;

		constructor(
			private readonly urlMapper: vdb.UrlMapper,
			userRepository: vdb.repositories.UserRepository,
			contract: dc.TagApiContract) {

			this.categoryName = ko.observable(contract.categoryName);
			this.defaultNameLanguage = ko.observable(contract.defaultNameLanguage);
			this.description = new globalization.EnglishTranslatedStringEditViewModel(contract.translatedDescription);
			this.id = contract.id;
			this.names = globalization.NamesEditViewModel.fromContracts(contract.names);
			this.parent = ko.observable(contract.parent);
			this.relatedTags = ko.observableArray(contract.relatedTags);
			this.targets = ko.observable(contract.targets);
			this.webLinks = new WebLinksEditViewModel(contract.webLinks);

			this.validationError_needDescription = ko.computed(() =>
				!this.description.original() &&
				_.isEmpty(this.webLinks.webLinks())
			);

			this.parentName = ko.computed(() => this.parent() ? this.parent().name : null);

			this.hasValidationErrors = ko.computed(() =>
				this.validationError_needDescription()
			);

			window.setInterval(() => userRepository.refreshEntryEdit(models.EntryType.Tag, contract.id), 10000);

		}

		public categoryName: KnockoutObservable<string>;
		public defaultNameLanguage: KnockoutObservable<string>;
		public description: globalization.EnglishTranslatedStringEditViewModel;
		public hasValidationErrors: KnockoutComputed<boolean>;
		private id: number;
		public names: globalization.NamesEditViewModel;
		public parent: KnockoutObservable<dc.TagBaseContract>;
		public parentName: KnockoutComputed<string>;
		public relatedTags: KnockoutObservableArray<dc.TagBaseContract>;
		public submitting = ko.observable(false);
		public targets: KnockoutObservable<models.EntryType>;
		public validationExpanded = ko.observable(false);
		public validationError_needDescription: KnockoutComputed<boolean>;
        public webLinks: WebLinksEditViewModel;

		public addRelatedTag = (tag: dc.TagBaseContract) => this.relatedTags.push(tag);		

		public allowRelatedTag = (tag: dc.TagBaseContract) => this.denySelf(tag) && _.every(this.relatedTags(), t => t.id !== tag.id);

		public deleteViewModel = new DeleteEntryViewModel(notes => {
			$.ajax(this.urlMapper.mapRelative("api/tags/" + this.id + "?hardDelete=false&notes=" + encodeURIComponent(notes)), {
				type: 'DELETE', success: () => {
					window.location.href = vdb.utils.EntryUrlMapper.details_tag(this.id);
				}
			});
		});

		public denySelf = (tag: dc.TagBaseContract) => (tag && tag.id !== this.id);

		public submit = () => {
			this.submitting(true);
			return true;
		}

		public hasTargetType = (target: models.EntryType) => {		
			const hasFlag = (t) => (this.targets() & t) === t;
			const checkFlags = () => {
				const types = [models.EntryType.Album, models.EntryType.Artist, models.EntryType.ReleaseEvent, models.EntryType.Song];
				if (this.targets() === _.sum(types)) {
					this.targets(TagEditViewModel.allEntryTypes);
				} else {
					this.targets(_.chain(types).filter(t => hasFlag(t)).sum().value());
				}
			};
			const addFlag = () => {
				this.targets(this.targets() | target);
				checkFlags();
			};
			const removeFlag = () => {
				if (hasFlag(target)) {
					this.targets(this.targets() - target);
					checkFlags();
				}
			};
			return ko.computed<boolean>({
				read: () => hasFlag(target),
				write: flag => flag ? addFlag() : removeFlag()
			});
		}

		public trashViewModel = new DeleteEntryViewModel(notes => {
			$.ajax(this.urlMapper.mapRelative("api/tags/" + this.id + "?hardDelete=true&notes=" + encodeURIComponent(notes)), {
				type: 'DELETE', success: () => {
					window.location.href = this.urlMapper.mapRelative("/Tag");
				}
			});
		});
	}

}