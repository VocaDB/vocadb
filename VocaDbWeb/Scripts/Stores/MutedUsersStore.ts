import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { LocalStorageStateStore } from '@/route-sphere';
import Ajv from 'ajv';
import { pull } from 'lodash-es';
import { action, computed, makeObservable, observable } from 'mobx';

import schema from './MutedUsersLocalStorageState.schema.json';

interface MutedUsersLocalStorageState {
	mutedUserIds?: number[];
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<MutedUsersLocalStorageState>(schema);

export class MutedUsersStore
	implements LocalStorageStateStore<MutedUsersLocalStorageState> {
	@observable mutedUsers: UserApiContract[] = [];

	constructor() {
		makeObservable(this);
	}

	@computed get mutedUserIds(): number[] {
		return this.mutedUsers.map((mutedUser) => mutedUser.id);
	}
	set mutedUserIds(value: number[]) {
		this.mutedUsers = value.map((mutedUserId) => ({ id: mutedUserId }));
	}

	@computed.struct get localStorageState(): MutedUsersLocalStorageState {
		return {
			mutedUserIds: this.mutedUserIds,
		};
	}
	set localStorageState(value: MutedUsersLocalStorageState) {
		this.mutedUserIds = value.mutedUserIds ?? [];
	}

	validateLocalStorageState(
		localStorageState: any,
	): localStorageState is MutedUsersLocalStorageState {
		return validate(localStorageState);
	}

	includes = (userId: number): boolean => {
		return this.mutedUserIds.includes(userId);
	};

	find = (userId: number): UserApiContract | undefined => {
		return this.mutedUsers.find((mutedUser) => mutedUser.id === userId);
	};

	@action addMutedUser = (userId: number): void => {
		this.mutedUsers.push({ id: userId });
	};

	@action removeMutedUser = (user: UserApiContract): void => {
		pull(this.mutedUsers, user);
	};
}
