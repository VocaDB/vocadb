import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { LocalStorageStateStore } from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import { pull } from 'lodash-es';
import { action, computed, makeObservable, observable } from 'mobx';

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
	@observable public mutedUsers: UserApiContract[] = [];

	public constructor() {
		makeObservable(this);
	}

	@computed public get mutedUserIds(): number[] {
		return this.mutedUsers.map((mutedUser) => mutedUser.id);
	}
	public set mutedUserIds(value: number[]) {
		this.mutedUsers = value.map((mutedUserId) => ({ id: mutedUserId }));
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

	public includes = (userId: number): boolean => {
		return this.mutedUserIds.includes(userId);
	};

	public find = (userId: number): UserApiContract | undefined => {
		return this.mutedUsers.find((mutedUser) => mutedUser.id === userId);
	};

	@action public addMutedUser = (userId: number): void => {
		this.mutedUsers.push({ id: userId });
	};

	@action public removeMutedUser = (user: UserApiContract): void => {
		pull(this.mutedUsers, user);
	};
}
