import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { ArtistFilters } from '@/Stores/Search/ArtistFilters';
import { TagFilters } from '@/Stores/Search/TagFilters';
import { LocalStorageStateStore } from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import { action, computed, makeObservable, observable } from 'mobx';

export interface SkipListLocalStorageState {
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
	@observable public dialogVisible = false;
	public readonly artistFilters: ArtistFilters;
	public readonly tagFilters: TagFilters;

	public constructor(
		values: GlobalValues,
		artistRepo: ArtistRepository,
		tagRepo: TagRepository,
	) {
		makeObservable(this);

		this.artistFilters = new ArtistFilters(values, artistRepo);
		this.tagFilters = new TagFilters(values, tagRepo);
	}

	@computed public get artistIds(): number[] {
		return this.artistFilters.artistIds;
	}
	public set artistIds(value: number[]) {
		this.artistFilters.artistIds = value;
	}

	@computed public get tagIds(): number[] {
		return this.tagFilters.tagIds;
	}
	public set tagIds(value: number[]) {
		// OPTIMIZE
		this.tagFilters.tags = [];
		this.tagFilters.addTags(value);
	}

	@computed.struct public get localStorageState(): SkipListLocalStorageState {
		return {
			artistIds: this.artistIds,
			tagIds: this.tagIds,
		};
	}
	public set localStorageState(value: SkipListLocalStorageState) {
		this.artistIds = value.artistIds ?? [];
		this.tagIds = value.tagIds ?? [];
	}

	public validateLocalStorageState = (
		localStorageState: any,
	): localStorageState is SkipListLocalStorageState => {
		return validate(localStorageState);
	};

	@action public showDialog = (): void => {
		this.dialogVisible = true;
	};

	@action public hideDialog = (): void => {
		this.dialogVisible = false;
	};
}
