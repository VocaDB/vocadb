import { ArtistAutoCompleteParams } from '../KnockoutExtensions/AutoCompleteParams';
import ArtistContract from '../DataContracts/Artist/ArtistContract';
import ArtistForAlbumContract from '../DataContracts/ArtistForAlbumContract';
import ArtistRepository from '../Repositories/ArtistRepository';
import ArtistRoles from '../Models/Artists/ArtistRoles';
import BasicEntryLinkViewModel from './BasicEntryLinkViewModel';
import DuplicateEntryResultContract from '../DataContracts/DuplicateEntryResultContract';
import EntryType from '../Models/EntryType';
import EntryUrlMapper from '../Shared/EntryUrlMapper';
import { SongAutoCompleteParams } from '../KnockoutExtensions/AutoCompleteParams';
import SongContract from '../DataContracts/Song/SongContract';
import SongHelper from '../Helpers/SongHelper';
import SongRepository from '../Repositories/SongRepository';
import SongType from '../Models/Songs/SongType';
import TagApiContract from '../DataContracts/Tag/TagApiContract';
import TagRepository from '../Repositories/TagRepository';

//module vdb.viewModels {

    // View model for song creation view
    export default class SongCreateViewModel {
        
        addArtist: (artistId: number) => void;

        artistSearchParams: ArtistAutoCompleteParams;

		artists = ko.observableArray<ArtistContract>([]);

		public artistsWithRoles: KnockoutComputed<ArtistForAlbumContract[]> = ko.computed(() => _.map(this.artists(), a => {
			return { artist: a, roles: ArtistRoles[ArtistRoles.Default] }
		}));

		private getArtistIds = () => {
			return _.map(this.artists(), a => a.id);
		}

		public checkDuplicatesAndPV = (vm?, event?: JQueryEventObject) => {
			this.checkDuplicates(vm, event, true);
		}

        public checkDuplicates = (vm?, event?: JQueryEventObject, getPVInfo = false) => {
	   
			var term1 = this.nameOriginal();
			var term2 = this.nameRomaji();
			var term3 = this.nameEnglish();
			var pv1 = this.pv1();
			var pv2 = this.pv2();
			var artists = this.getArtistIds();

			this.songRepository.findDuplicate(
				{ term: [ term1, term2, term3 ], pv: [ pv1, pv2 ], artistIds: artists, getPVInfo: getPVInfo },
				result => {

                this.dupeEntries(result.matches);

				if (result.title && !this.hasName()) {

					if (result.titleLanguage === "English") {
						this.nameEnglish(result.title);
					} else {
						this.nameOriginal(result.title);
					}

                }

                if (result.songType && result.songType !== "Unspecified" && this.songType() === "Original") {
                    this.songType(result.songType);
                }

                if (result.artists && this.artists().length === 0) {

                    _.forEach(result.artists, artist => {
                        this.artists.push(artist);
                    });

                }

				});

			if (event)
				event.preventDefault();

			return false;
			 
		}

		private getSongTypeTag = async (songType: string) => {
			const tag = await this.tagRepository.getEntryTypeTag(EntryType.Song, songType);
			this.songTypeTag(tag);
		}

        dupeEntries = ko.observableArray<DuplicateEntryResultContract>([]);

        isDuplicatePV: KnockoutComputed<boolean>;

        nameOriginal = ko.observable("");
        nameRomaji = ko.observable("");
        nameEnglish = ko.observable("");

		originalSongSuggestions: KnockoutComputed<DuplicateEntryResultContract[]>;

		originalVersion: BasicEntryLinkViewModel<SongContract>;
		originalVersionSearchParams: SongAutoCompleteParams;

        pv1 = ko.observable("");
        pv2 = ko.observable("");
		songType = ko.observable("Original");
		songTypeTag = ko.observable<TagApiContract>(null);
		songTypeName = ko.computed(() => this.songTypeTag()?.name);
		songTypeInfo = ko.computed(() => this.songTypeTag()?.description);
		songTypeTagUrl = ko.computed(() => EntryUrlMapper.details_tag_contract(this.songTypeTag()));

		canHaveOriginalVersion = ko.computed(() => SongType[this.songType()] !== SongType.Original);

        hasName: KnockoutComputed<boolean>;

		selectOriginal = (dupe: DuplicateEntryResultContract) => {
			this.songRepository.getOne(dupe.entry.id, song => this.originalVersion.entry(song));
		}

        public submit = () => {
            this.submitting(true);
            return true;
        };

        public submitting = ko.observable(false);

        removeArtist: (artist: ArtistContract) => void;

		constructor(
			private readonly songRepository: SongRepository,
			artistRepository: ArtistRepository,
			private readonly tagRepository: TagRepository,
			data?) {

            if (data) {
                this.nameOriginal(data.nameOriginal || "");
                this.nameRomaji(data.nameRomaji || "");
                this.nameEnglish(data.nameEnglish || "");
                this.pv1(data.pvUrl || "");
                this.pv2(data.reprintPVUrl || "");
                this.artists(data.artists || []);
            }

            this.addArtist = (artistId: number) => {

                if (artistId) {
                    artistRepository.getOne(artistId, artist => {
                        this.artists.push(artist);
						this.checkDuplicates();
                    });
                }

            };

            this.artistSearchParams = {
                acceptSelection: this.addArtist,
                height: 300
            };

            this.hasName = ko.computed(() => {
                return this.nameOriginal().length > 0 || this.nameRomaji().length > 0 || this.nameEnglish().length > 0;
            });

            this.isDuplicatePV = ko.computed(() => {
                return _.some(this.dupeEntries(), item => { return item.matchProperty === 'PV' });
            });
            
			this.originalVersion = new BasicEntryLinkViewModel<SongContract>(null, songRepository.getOne);

			this.originalVersionSearchParams = {
				acceptSelection: this.originalVersion.id,
				extraQueryParams: { songTypes: SongHelper.originalVersionTypesString() }
			};

			this.originalSongSuggestions = ko.computed(() => {

				if (!this.dupeEntries() || this.dupeEntries().length === 0)
					return [];

				return _.take(this.dupeEntries(), 3);

			});

            this.removeArtist = (artist: ArtistContract) => {
                this.artists.remove(artist);
            };
            
            if (this.pv1()) {
                this.checkDuplicatesAndPV();
			}

			this.songType.subscribe(this.getSongTypeTag);
			this.getSongTypeTag(this.songType());

        }
    
    }

//}