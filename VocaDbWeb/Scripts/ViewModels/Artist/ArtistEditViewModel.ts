/// <reference path="../../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../../DataContracts/WebLinkContract.ts" />
/// <reference path="../WebLinksEditViewModel.ts" />

module vdb.viewModels {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

    export class ArtistEditViewModel {

		public addAssociatedArtist = () => {

			if (this.newAssociatedArtist.isEmpty())
				return;

			this.associatedArtists.push(new ArtistForArtistEditViewModel({ parent: this.newAssociatedArtist.entry(), linkType: this.newAssociatedArtistType() }));

			this.newAssociatedArtist.clear();

		}

		private addGroup = (artistId: number) => {
			
			if (artistId) {
				this.artistRepo.getOne(artistId, (artist: dc.ArtistContract) => {
					this.groups.push({ id: 0, parent: artist });
				});
			}

		}

		public artistType: KnockoutComputed<cls.artists.ArtistType>;
		public artistTypeStr: KnockoutObservable<string>;
		public allowBaseVoicebank: KnockoutComputed<boolean>;
		public associatedArtists: KnockoutObservableArray<ArtistForArtistEditViewModel>;
		public baseVoicebank: BasicEntryLinkViewModel<dc.ArtistContract>;
		public baseVoicebankSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;
		public canHaveCircles: KnockoutComputed<boolean>;
		// Can have related artists (associatedArtists) such as voice provider and illustrator
		public canHaveRelatedArtists: KnockoutComputed<boolean>;
		public canHaveReleaseDate: KnockoutComputed<boolean>;

		// Clears fields that are not valid for the selected artist type.
		private clearInvalidData = () => {

			if (!this.canHaveReleaseDate()) {
				this.releaseDate(null);				
			}

			if (!this.canHaveRelatedArtists()) {
				this.associatedArtists([]);
			}

			if (!this.allowBaseVoicebank()) {
				this.baseVoicebank.clear();
			}

			if (!this.canHaveCircles()) {
				this.groups([]);
			}

		}

		public defaultNameLanguage: KnockoutObservable<string>;

		public deleteViewModel = new DeleteEntryViewModel(notes => {
			$.ajax(this.urlMapper.mapRelative("api/artists/" + this.id + "?notes=" + encodeURIComponent(notes)), {
				type: 'DELETE', success: () => {
					window.location.href = this.urlMapper.mapRelative("/Artist/Details/" + this.id);
				}
			});
		});

		public description: globalization.EnglishTranslatedStringEditViewModel;
		public groups: KnockoutObservableArray<dc.artists.ArtistForArtistContract>;

		public groupSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams = {
			acceptSelection: this.addGroup,
			extraQueryParams: { artistTypes: "Label,Circle,OtherGroup,Band" },
			height: 300
		};

		public hasValidationErrors: KnockoutComputed<boolean>;
		public id: number;
		public illustrator: BasicEntryLinkViewModel<dc.ArtistContract>;

		public names: globalization.NamesEditViewModel;
		public newAssociatedArtist: BasicEntryLinkViewModel<dc.ArtistContract>;
		public newAssociatedArtistType = ko.observable<string>();
		public pictures: EntryPictureFileListEditViewModel;
		public releaseDate: KnockoutObservable<Date>;

		public removeGroup = (group: dc.artists.ArtistForArtistContract) => {
			this.groups.remove(group);
		}

		public status: KnockoutObservable<string>;

		public submit = () => {

			if (this.hasValidationErrors() && this.status() !== "Draft"
				&& this.dialogService.confirm(vdb.resources.entryEdit.saveWarning) === false) {

				return false;

			}

			this.clearInvalidData();

			this.submitting(true);

			var submittedModel: dc.artists.ArtistForEditContract = {
				artistType: this.artistTypeStr(),
				associatedArtists: _.map(this.associatedArtists(), a => a.toContract()),
				baseVoicebank: this.baseVoicebank.entry(),
				defaultNameLanguage: this.defaultNameLanguage(),
				description: this.description.toContract(),
				groups: this.groups(),
				id: this.id,
				illustrator: this.illustrator.entry(),
				names: this.names.toContracts(),
				pictures: this.pictures.toContracts(),
				releaseDate: (this.releaseDate() ? this.releaseDate().toISOString() : null),
				status: this.status(),
				updateNotes: this.updateNotes(),
				voiceProvider: this.voiceProvider.entry(),
				webLinks: this.webLinks.toContracts(),
				pictureMime: ""
			};

			this.submittedJson(ko.toJSON(submittedModel));

			return true;

		}

		public submittedJson = ko.observable("");

		public submitting = ko.observable(false);
		public updateNotes = ko.observable("");
		public validationExpanded = ko.observable(false);
		public validationError_needReferences: KnockoutComputed<boolean>;
		public validationError_needType: KnockoutComputed<boolean>;
		public validationError_unnecessaryPName: KnockoutComputed<boolean>;
		public validationError_unspecifiedNames: KnockoutComputed<boolean>;
		public voiceProvider: BasicEntryLinkViewModel<dc.ArtistContract>;
        public webLinks: WebLinksEditViewModel;

        constructor(
			private artistRepo: rep.ArtistRepository,
			userRepository: rep.UserRepository,
			private urlMapper: vdb.UrlMapper,
			webLinkCategories: vdb.dataContracts.TranslatedEnumField[],
			data: dc.artists.ArtistForEditContract,
			private dialogService: vdb.ui_dialog.IDialogService) {

			this.artistTypeStr = ko.observable(data.artistType);
			this.artistType = ko.computed(() => cls.artists.ArtistType[this.artistTypeStr()]);
			this.allowBaseVoicebank = ko.computed(() => helpers.ArtistHelper.isVocalistType(this.artistType()) || this.artistType() == cls.artists.ArtistType.OtherIndividual);
			this.associatedArtists = ko.observableArray(_.map(data.associatedArtists, a => new ArtistForArtistEditViewModel(a)));
			this.baseVoicebank = new BasicEntryLinkViewModel(data.baseVoicebank, artistRepo.getOne);
			this.description = new globalization.EnglishTranslatedStringEditViewModel(data.description);
			this.defaultNameLanguage = ko.observable(data.defaultNameLanguage);
			this.groups = ko.observableArray(data.groups);
			this.id = data.id;
			this.illustrator = new BasicEntryLinkViewModel(data.illustrator, artistRepo.getOne);
			this.names = globalization.NamesEditViewModel.fromContracts(data.names);
			this.newAssociatedArtist = new BasicEntryLinkViewModel(null, artistRepo.getOne);
			this.pictures = new EntryPictureFileListEditViewModel(data.pictures);
			this.releaseDate = ko.observable(data.releaseDate ? moment(data.releaseDate).toDate() : null); // Assume server date is UTC
			this.status = ko.observable(data.status);
			this.voiceProvider = new BasicEntryLinkViewModel(data.voiceProvider, artistRepo.getOne);
            this.webLinks = new WebLinksEditViewModel(data.webLinks, webLinkCategories);
    
			this.baseVoicebankSearchParams = {
				acceptSelection: this.baseVoicebank.id,
				extraQueryParams: { artistTypes: "Vocaloid,UTAU,CeVIO,OtherVocalist,OtherVoiceSynthesizer,Unknown" },
				ignoreId: this.id,
			};

			this.canHaveCircles = ko.computed(() => {
				return this.artistType() !== cls.artists.ArtistType.Label;
			});

			this.canHaveRelatedArtists = ko.computed(() => {
				return helpers.ArtistHelper.isVocalistType(this.artistType());
			});

			this.canHaveReleaseDate = ko.computed(() => {
				const vocaloidTypes = [
					cls.artists.ArtistType.Vocaloid, cls.artists.ArtistType.UTAU,
					cls.artists.ArtistType.CeVIO, cls.artists.ArtistType.OtherVoiceSynthesizer
				];
				return _.includes(vocaloidTypes, this.artistType());
			});

			this.newAssociatedArtist.entry.subscribe(this.addAssociatedArtist);

			this.validationError_needReferences = ko.computed(() => (this.description.original() == null || this.description.original().length) == 0 && this.webLinks.webLinks().length == 0);
			this.validationError_needType = ko.computed(() => this.artistType() === cls.artists.ArtistType.Unknown);
			this.validationError_unspecifiedNames = ko.computed(() => !this.names.hasPrimaryName());
			this.validationError_unnecessaryPName = ko.computed(() => {
				var allNames = this.names.getAllNames();
				return _.some(allNames, n =>
					_.some(allNames, n2 => n !== n2
					&& (n.value() === n2.value() + "P"
						|| n.value() === n2.value() + "-P"
						|| n.value() === n2.value() + "p"
						|| n.value() === n2.value() + "-p")));
			});

			this.hasValidationErrors = ko.computed(() =>
				this.validationError_needReferences() ||
				this.validationError_needType() ||
				this.validationError_unspecifiedNames() ||
				this.validationError_unnecessaryPName()
			);
			    
			window.setInterval(() => userRepository.refreshEntryEdit(models.EntryType.Artist, data.id), 10000);			

        }

    }

	export class ArtistForArtistEditViewModel {

		constructor(link: dc.artists.ArtistForArtistContract) {
			this.linkType = ko.observable(link.linkType);
			this.parent = link.parent;
		}

		public linkType: KnockoutObservable<string>;

		public parent: dc.ArtistContract;

		public toContract = () => {
			return {
				linkType: this.linkType(), parent: this.parent
			} as dc.artists.ArtistForArtistContract;
		}

	}

}