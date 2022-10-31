import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { LocalStorageStateStore } from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import { computed, makeObservable, observable } from 'mobx';

interface MutedUsersLocalStorageState {
	mutedUserIds?: number[];
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<MutedUsersLocalStorageState> = require('./MutedUsersLocalStorageState.schema');
const validate = ajv.compile(schema);

export class MutedUsersStore
	implements LocalStorageStateStore<MutedUsersLocalStorageState> {
	@observable public mutedUserIds: number[] = [];

	public constructor() {
		makeObservable(this);
	}

	@computed.struct public get localStorageState(): MutedUsersLocalStorageState {
		return {
			mutedUserIds: this.mutedUserIds,
		};
	}
	public set localStorageState(value: MutedUsersLocalStorageState) {
		this.mutedUserIds = value.mutedUserIds ?? [];
	}

	public validateLocalStorageState(
		localStorageState: any,
	): localStorageState is MutedUsersLocalStorageState {
		return validate(localStorageState);
	}

	public includes = (user: UserApiContract | undefined): boolean => {
		return user !== undefined && this.mutedUserIds.includes(user.id);
	};
}
