
import ResourceRepository from '../../Repositories/ResourceRepository';
import PartialFindResultContract from '../../DataContracts/PartialFindResultContract';
import SongListContract from '../../DataContracts/Song/SongListContract';
import SongListRepository from '../../Repositories/SongListRepository';
import SongListsBaseViewModel from './SongListsBaseViewModel';
import TagRepository from '../../Repositories/TagRepository';

//module vdb.viewModels.songList {
	
	export class FeaturedSongListsViewModel {

		constructor(listRepo: SongListRepository,
			resourceRepo: ResourceRepository,
			tagRepo: TagRepository,
			languageSelection: string,
			cultureCode: string,
			tagIds: number[],
			categoryNames: string[]) {
			
			_.forEach(categoryNames, (categoryName) => {
				this.categories[categoryName] = new FeaturedSongListCategoryViewModel(listRepo, resourceRepo, tagRepo, languageSelection, cultureCode, tagIds, categoryName);
			});

		}

		public categories: { [index: string]: FeaturedSongListCategoryViewModel; } = {};

	}

	export class FeaturedSongListCategoryViewModel extends SongListsBaseViewModel {

		constructor(private listRepo: SongListRepository,
			resourceRepo: ResourceRepository,
			tagRepo: TagRepository,
			languageSelection: string,
			cultureCode: string,
			tagIds: number[],
			private category: string) {

			// Should figure out a better way for this.
			super(resourceRepo, tagRepo, languageSelection, cultureCode, tagIds, category === "Concerts" || category === "VocaloidRanking");

		}

		public loadMoreItems = (callback: (result: PartialFindResultContract<SongListContract>) => void) => {
			this.listRepo.getFeatured(this.query(), this.category, { start: this.start, maxEntries: 50, getTotalCount: true }, this.tagFilters.tagIds(), this.sort(), callback);
		};

	}

//}