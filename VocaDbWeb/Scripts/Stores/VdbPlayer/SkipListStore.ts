import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { ArtistFilters } from '@/Stores/Search/ArtistFilters';
import { TagFilters } from '@/Stores/Search/TagFilters';
import { PlayQueueEntryContract } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { includesAny, LocalStorageStateStore } from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import { action, computed, makeObservable, observable } from 'mobx';

export interface SkipListLocalStorageState {
	removeFromPlayQueueOnSkip?: boolean;
	artistIds?: number[];
	tagIds?: number[];
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<SkipListLocalStorageState> = require('./SkipListLocalStorageState.schema');
const validate = ajv.compile(schema);

export class SkipListStore
	implements LocalStorageStateStore<SkipListLocalStorageState> {
	@observable dialogVisible = false;
	@observable removeFromPlayQueueOnSkip = false;
	readonly artistFilters: ArtistFilters;
	readonly tagFilters: TagFilters;

	constructor(
		values: GlobalValues,
		artistRepo: ArtistRepository,
		tagRepo: TagRepository,
	) {
		makeObservable(this);

		this.artistFilters = new ArtistFilters(values, artistRepo);
		this.tagFilters = new TagFilters(values, tagRepo);
	}

	@computed get artistIds(): number[] {
		return this.artistFilters.artistIds;
	}
	set artistIds(value: number[]) {
		this.artistFilters.artistIds = value;
	}

	@computed get tagIds(): number[] {
		return this.tagFilters.tagIds;
	}
	set tagIds(value: number[]) {
		// OPTIMIZE
		this.tagFilters.tags = [];
		this.tagFilters.addTags(value);
	}

	@computed.struct get localStorageState(): SkipListLocalStorageState {
		return {
			removeFromPlayQueueOnSkip: this.removeFromPlayQueueOnSkip,
			artistIds: this.artistIds,
			tagIds: this.tagIds,
		};
	}
	set localStorageState(value: SkipListLocalStorageState) {
		this.removeFromPlayQueueOnSkip = value.removeFromPlayQueueOnSkip ?? false;
		this.artistIds = value.artistIds ?? [];
		this.tagIds = value.tagIds ?? [];
	}

	validateLocalStorageState = (
		localStorageState: any,
	): localStorageState is SkipListLocalStorageState => {
		return validate(localStorageState);
	};

	@action showDialog = (): void => {
		this.dialogVisible = true;
	};

	@action hideDialog = (): void => {
		this.dialogVisible = false;
	};

	includesAny = ({ artistIds, tagIds }: PlayQueueEntryContract): boolean => {
		return (
			includesAny(this.artistIds, artistIds) || includesAny(this.tagIds, tagIds)
		);
	};
}
