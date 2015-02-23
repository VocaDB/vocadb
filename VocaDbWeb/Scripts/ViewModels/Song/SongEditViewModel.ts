/// <reference path="../../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../../DataContracts/WebLinkContract.ts" />
/// <reference path="../WebLinksEditViewModel.ts" />

module vdb.viewModels {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import hel = vdb.helpers;
	import rep = vdb.repositories;
	var SongType = cls.songs.SongType;

    export class SongEditViewModel {

        // List of artist links for this song.
        public artistLinks: KnockoutObservableArray<ArtistForAlbumEditViewModel>;
		artistSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;
		public canHaveOriginalVersion: KnockoutComputed<boolean>;
		public defaultNameLanguage: KnockoutObservable<string>;
		public deleted: boolean;
		public id: number;
        public length: KnockoutObservable<number>;
		public lengthFormatted: KnockoutComputed<string>;
		public lyrics: songs.LyricsForSongListEditViewModel;
		public names: globalization.NamesEditViewModel;
		public notes: globalization.EnglishTranslatedStringEditViewModel;
		public originalVersion: BasicEntryLinkViewModel<dc.SongContract>;
		public originalVersionSearchParams: vdb.knockoutExtensions.SongAutoCompleteParams;
		public pvs: pvs.PVListEditViewModel;
		public songType: KnockoutComputed<cls.songs.SongType>;
		public songTypeStr: KnockoutObservable<string>;
		public status: KnockoutObservable<string>;
		public submittedJson = ko.observable("");
		public submitting = ko.observable(false);
		private tags: string[];
		public updateNotes = ko.observable("");
		public validationExpanded = ko.observable(false);
        public webLinks: WebLinksEditViewModel;

		// Adds a new artist to the album
		// artistId: Id of the artist being added, if it's an existing artist. Can be null, if custom artist.
		// customArtistName: Name of the custom artist being added. Can be null, if existing artist.
		addArtist = (artistId?: number, customArtistName?: string) => {

			if (artistId) {

				this.artistRepository.getOne(artistId, artist => {

					var data: dc.ArtistForAlbumContract = {
						artist: artist,
						isSupport: false,
						name: artist.name,
						id: 0,
						roles: 'Default'
					};

					var link = new ArtistForAlbumEditViewModel(null, data);
					this.artistLinks.push(link);

				});

			} else {

				var data: dc.ArtistForAlbumContract = {
					artist: null,
					name: customArtistName,
					isSupport: false,
					id: 0,
					roles: 'Default'
				};

				var link = new ArtistForAlbumEditViewModel(null, data);
				this.artistLinks.push(link);

			}

		};

		public artistRolesEditViewModel: artists.ArtistRolesEditViewModel;

		public editArtistRoles = (artist: ArtistForAlbumEditViewModel) => {
			this.artistRolesEditViewModel.show(artist);
		}

		// Removes an artist from this album.
		public removeArtist = (artist: ArtistForAlbumEditViewModel) => {
			this.artistLinks.remove(artist);
		};

		public submit = () => {

			this.submitting(true);

			var submittedModel: dc.songs.SongForEditContract = {
				artists: _.map(this.artistLinks(), artist => artist.toContract()),
				defaultNameLanguage: this.defaultNameLanguage(),
				deleted: this.deleted,
				id: this.id,
				lengthSeconds: this.length(),
				lyrics: this.lyrics.toContracts(),
				names: this.names.toContracts(),
				notes: this.notes.toContract(),
				originalVersion: this.originalVersion.entry(),
				pvs: this.pvs.toContracts(),
				songType: this.songTypeStr(),
				status: this.status(),
				tags: this.tags,
				updateNotes: this.updateNotes(),
				webLinks: this.webLinks.toContracts()
			};

			this.submittedJson(ko.toJSON(submittedModel));

			return true;

        }

		public translateArtistRole = (role: string) => {
			return this.artistRoleNames[role];
		};

		public hasValidationErrors: KnockoutComputed<boolean>;
		public validationError_needArtist: KnockoutComputed<boolean>;
		public validationError_needProducer: KnockoutComputed<boolean>;
		public validationError_needType: KnockoutComputed<boolean>;
		public validationError_nonInstrumentalSongNeedsVocalists: KnockoutComputed<boolean>;
		public validationError_unspecifiedNames: KnockoutComputed<boolean>;

		constructor(
			songRepository: rep.SongRepository,
			private artistRepository: rep.ArtistRepository,
			pvRepository: rep.PVRepository,
			urlMapper: vdb.UrlMapper,
			private artistRoleNames: { [key: string]: string; },
			webLinkCategories: vdb.dataContracts.TranslatedEnumField[],
			data: dc.songs.SongForEditContract,
			canBulkDeletePVs: boolean) {

			this.artistLinks = ko.observableArray(_.map(data.artists, artist => new ArtistForAlbumEditViewModel(null, artist)));
			this.defaultNameLanguage = ko.observable(data.defaultNameLanguage);
			this.deleted = data.deleted;
			this.id = data.id;
			this.length = ko.observable(data.lengthSeconds);
			this.lyrics = new songs.LyricsForSongListEditViewModel(data.lyrics);
			this.names = globalization.NamesEditViewModel.fromContracts(data.names);
			this.notes = new globalization.EnglishTranslatedStringEditViewModel(data.notes);
			this.originalVersion = new BasicEntryLinkViewModel<dc.SongContract>(data.originalVersion, songRepository.getOne);
			this.pvs = new pvs.PVListEditViewModel(pvRepository, urlMapper, data.pvs, canBulkDeletePVs);
			this.songTypeStr = ko.observable(data.songType);
			this.songType = ko.computed(() => cls.songs.SongType[this.songTypeStr()]);
			this.status = ko.observable(data.status);
			this.tags = data.tags;
			this.webLinks = new WebLinksEditViewModel(data.webLinks, webLinkCategories);


			this.artistRolesEditViewModel = new artists.ArtistRolesEditViewModel(artistRoleNames);

			this.artistSearchParams = {
				createNewItem: vdb.resources.song.addExtraArtist,
				acceptSelection: this.addArtist,
				height: 300
			};

			this.canHaveOriginalVersion = ko.computed(() => this.songType() != cls.songs.SongType.Original);

			this.originalVersionSearchParams = {
				acceptSelection: this.originalVersion.id,
				allowCreateNew: false,
				extraQueryParams: {
					songTypes: "Unspecified,Original,Remaster,Remix,Cover,Mashup,DramaPV,Other"
				},
				ignoreId: this.id,
				height: 250
			};

            
            this.lengthFormatted = ko.computed({
				read: () => {
					return vdb.helpers.DateTimeHelper.formatFromSeconds(this.length());
                },
                write: (value: string) => {
                    var parts = value.split(":");
                    if (parts.length == 2 && parseInt(parts[0], 10) != NaN && parseInt(parts[1], 10) != NaN) {
                        this.length(parseInt(parts[0], 10) * 60 + parseInt(parts[1], 10));
                    } else if (parts.length == 1 && !isNaN(parseInt(parts[0], 10))) {
                        this.length(parseInt(parts[0], 10));
                    } else {
                        this.length(0);
                    }
                }
			});

			this.validationError_needArtist = ko.computed(() => !_.some(this.artistLinks(), a => a.artist != null));
			this.validationError_needProducer = ko.computed(() => !this.validationError_needArtist() && !_.some(this.artistLinks(), a => a.artist != null && hel.ArtistHelper.isProducerRole(a.artist, a.rolesArray(), hel.SongHelper.isAnimation(this.songType()))));
			this.validationError_needType = ko.computed(() => this.songType() == SongType.Unspecified);

			this.validationError_nonInstrumentalSongNeedsVocalists = ko.computed(() => {

				return (!this.validationError_needArtist()
					&& !hel.SongHelper.isInstrumental(this.songType())
					&& !_.some(this.tags, t => t == cls.tags.Tag.commonTag_instrumental))
					&& !_.some(this.artistLinks(), a => hel.ArtistHelper.isVocalistRole(a.artist, a.rolesArray()));

			});

			this.validationError_unspecifiedNames = ko.computed(() => !this.names.hasPrimaryName());

			this.hasValidationErrors = ko.computed(() =>
				this.validationError_needArtist() ||
				this.validationError_needProducer() ||
				this.validationError_needType() ||
				this.validationError_nonInstrumentalSongNeedsVocalists() ||
				this.validationError_unspecifiedNames()
			);

        }

    }

}