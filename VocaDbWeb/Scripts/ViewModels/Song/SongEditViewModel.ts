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

		public albumReleaseDate: moment.Moment;
        // List of artist links for this song.
        public artistLinks: KnockoutObservableArray<ArtistForAlbumEditViewModel>;
		artistSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;
		public canHaveOriginalVersion: KnockoutComputed<boolean>;
		public defaultNameLanguage: KnockoutObservable<string>;
		public deleted: boolean;
		public eventDate: KnockoutComputed<moment.Moment>;
		public firstPvDate: KnockoutComputed<moment.Moment>;
		public id: number;
        public length: KnockoutObservable<number>;
		public lengthFormatted: KnockoutComputed<string>;
		public lyrics: songs.LyricsForSongListEditViewModel;
		public names: globalization.NamesEditViewModel;
		public notes: globalization.EnglishTranslatedStringEditViewModel;
		public originalVersion: BasicEntryLinkViewModel<dc.SongContract>;
		public originalVersionSearchParams: vdb.knockoutExtensions.SongAutoCompleteParams;
		public publishDate: KnockoutObservable<Date>;
		public pvs: pvs.PVListEditViewModel;
		public releaseEvent: BasicEntryLinkViewModel<dc.ReleaseEventContract>;
		public songType: KnockoutComputed<cls.songs.SongType>;
		public songTypeStr: KnockoutObservable<string>;
		public status: KnockoutObservable<string>;
		public submittedJson = ko.observable("");
		public submitting = ko.observable(false);
		private tags: number[];
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

		public deleteViewModel = new DeleteEntryViewModel(notes => {
			$.ajax(this.urlMapper.mapRelative("api/songs/" + this.id + "?notes=" + encodeURIComponent(notes)), { type: 'DELETE', success: () => {
				window.location.href = this.urlMapper.mapRelative("/Song/Details/" + this.id);
			}});
		});

		public editArtistRoles = (artist: ArtistForAlbumEditViewModel) => {
			this.artistRolesEditViewModel.show(artist);
		}

		private hasAlbums: boolean;

		// Removes an artist from this album.
		public removeArtist = (artist: ArtistForAlbumEditViewModel) => {
			this.artistLinks.remove(artist);
		};

		public submit = () => {

			if (this.hasValidationErrors() && this.status() !== "Draft"
				&& this.dialogService.confirm(vdb.resources.entryEdit.saveWarning) === false) {
				
				return false;

			}

			this.submitting(true);

			var submittedModel: dc.songs.SongForEditContract = {
				artists: _.map(this.artistLinks(), artist => artist.toContract()),
				defaultNameLanguage: this.defaultNameLanguage(),
				deleted: this.deleted,
				hasAlbums: this.hasAlbums,
				id: this.id,
				lengthSeconds: this.length(),
				lyrics: this.lyrics.toContracts(),
				names: this.names.toContracts(),
				notes: this.notes.toContract(),
				originalVersion: this.originalVersion.entry(),
				publishDate: (this.publishDate() ? this.publishDate().toISOString() : null),
				pvs: this.pvs.toContracts(),
				releaseEvent: this.releaseEvent.entry(),
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
		public showInstrumentalNote: KnockoutComputed<boolean>;
		public validationError_duplicateArtist: KnockoutComputed<boolean>;
		public validationError_needArtist: KnockoutComputed<boolean>;
		public validationError_needOriginal: KnockoutComputed<boolean>;
		public validationError_needProducer: KnockoutComputed<boolean>;
		public validationError_needReferences: KnockoutComputed<boolean>;
		public validationError_needType: KnockoutComputed<boolean>;
		public validationError_nonInstrumentalSongNeedsVocalists: KnockoutComputed<boolean>;
		public validationError_unspecifiedNames: KnockoutComputed<boolean>;

		constructor(
			songRepository: rep.SongRepository,
			private artistRepository: rep.ArtistRepository,
			pvRepository: rep.PVRepository,
			userRepository: rep.UserRepository,
			private urlMapper: vdb.UrlMapper,
			private artistRoleNames: { [key: string]: string; },
			webLinkCategories: vdb.dataContracts.TranslatedEnumField[],
			data: dc.songs.SongForEditContract,
			canBulkDeletePVs: boolean,
			private dialogService: ui_dialog.IDialogService,
			private instrumentalTagId: number,
			public languageNames) {

			this.albumReleaseDate = data.albumReleaseDate ? moment(data.albumReleaseDate) : null;
			this.artistLinks = ko.observableArray(_.map(data.artists, artist => new ArtistForAlbumEditViewModel(null, artist)));
			this.defaultNameLanguage = ko.observable(data.defaultNameLanguage);
			this.deleted = data.deleted;
			this.id = data.id;
			this.length = ko.observable(data.lengthSeconds);
			this.lyrics = new songs.LyricsForSongListEditViewModel(data.lyrics);
			this.names = globalization.NamesEditViewModel.fromContracts(data.names);
			this.notes = new globalization.EnglishTranslatedStringEditViewModel(data.notes);
			this.originalVersion = new BasicEntryLinkViewModel<dc.SongContract>(data.originalVersion, songRepository.getOne);
			this.publishDate = ko.observable(data.publishDate ? moment(data.publishDate).toDate() : null); // Assume server date is UTC
			this.pvs = new pvs.PVListEditViewModel(pvRepository, urlMapper, data.pvs, canBulkDeletePVs, true);
			this.releaseEvent = new BasicEntryLinkViewModel<dc.ReleaseEventContract>(data.releaseEvent, null);
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

			this.canHaveOriginalVersion = ko.computed(() => this.songType() !== cls.songs.SongType.Original);

			this.hasAlbums = data.hasAlbums;

			this.originalVersionSearchParams = {
				acceptSelection: this.originalVersion.id,
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

			this.showInstrumentalNote = ko.computed(() => {
				return this.pvs.isPossibleInstrumental()
					&& this.songType() !== models.songs.SongType.Instrumental
					&& !_.some(this.tags, t => t === this.instrumentalTagId);
			});

			this.validationError_duplicateArtist = ko.computed(() => {
				return _.some(_.groupBy(this.artistLinks(), a => (a.artist ? a.artist.id.toString() : a.name) + a.isSupport()), a => a.length > 1);
			});

			this.validationError_needArtist = ko.computed(() => !_.some(this.artistLinks(), a => a.artist != null));

			this.validationError_needOriginal = ko.computed(() => {
				
				var songType = models.songs.SongType;
				var derivedTypes = [songType.Remaster, songType.Cover, songType.Instrumental, songType.MusicPV, songType.Other, songType.Remix];
				return (this.notes.original() === null || this.notes.original() === "")
					&& this.originalVersion.entry() == null
					&& _.includes(derivedTypes, this.songType());

			});

			this.validationError_needProducer = ko.computed(() => !this.validationError_needArtist() && !_.some(this.artistLinks(), a => a.artist != null && hel.ArtistHelper.isProducerRole(a.artist, a.rolesArray(), hel.SongHelper.isAnimation(this.songType()))));

			this.validationError_needReferences = ko.computed(() =>
				!this.hasAlbums
				&& _.isEmpty(this.notes.original())
				&& _.isEmpty(this.webLinks.webLinks())
				&& _.isEmpty(this.pvs.pvs()));

			this.validationError_needType = ko.computed(() => this.songType() === SongType.Unspecified);

			this.validationError_nonInstrumentalSongNeedsVocalists = ko.computed(() => {

				return (!this.validationError_needArtist()
					&& !hel.SongHelper.isInstrumental(this.songType())
					&& !_.some(this.tags, t => t === this.instrumentalTagId))
					&& !_.some(this.artistLinks(), a => hel.ArtistHelper.isVocalistRole(a.artist, a.rolesArray()));

			});

			this.validationError_unspecifiedNames = ko.computed(() => !this.names.hasPrimaryName());

			this.hasValidationErrors = ko.computed(() =>
				this.validationError_duplicateArtist() ||
				this.validationError_needArtist() ||
				this.validationError_needOriginal() ||
				this.validationError_needProducer() ||
				this.validationError_needReferences() ||
				this.validationError_needType() ||
				this.validationError_nonInstrumentalSongNeedsVocalists() ||
				this.validationError_unspecifiedNames()
			);
		
			this.eventDate = ko.computed(() => (this.releaseEvent.entry() && this.releaseEvent.entry().date ? moment(this.releaseEvent.entry().date) : null));

			this.firstPvDate = ko.computed(() => {

				var val = _.chain(this.pvs.pvs())
					.filter(pv => pv.publishDate && pv.pvType === models.pvs.PVType[models.pvs.PVType.Original])
					.map(pv => moment(pv.publishDate))
					.sortBy(p => p)
					.head<_.LoDashExplicitObjectWrapper<moment.Moment>>() 
					.value();

				return val;

			});

			window.setInterval(() => userRepository.refreshEntryEdit(models.EntryType.Song, data.id), 10000);

        }

    }

}