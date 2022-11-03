import { ArtistForUserForApiContract } from '@/DataContracts/User/ArtistForUserForApiContract';
import { UserWithPermissionsForApiContract } from '@/DataContracts/User/UserWithPermissionsForApiContract';
import { PermissionToken } from '@/Models/LoginManager';
import { UserGroup } from '@/Models/Users/UserGroup';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { pull } from 'lodash-es';
import { action, makeObservable, observable, runInAction } from 'mobx';

export class PermissionEditStore {
	@observable hasFlag: boolean;

	constructor(
		hasFlag: boolean,
		readonly hasPermission: boolean,
		readonly permissionToken: PermissionToken,
	) {
		makeObservable(this);

		this.hasFlag = hasFlag;
	}
}

export class UserEditStore {
	@observable active: boolean;
	@observable email: string;
	@observable errors?: Record<string, string[]>;
	@observable groupId: UserGroup;
	@observable name: string;
	@observable ownedArtists: ArtistForUserForApiContract[];
	@observable permissions: PermissionEditStore[];
	@observable poisoned: boolean;
	@observable submitting = false;
	@observable supporter: boolean;

	constructor(
		readonly contract: UserWithPermissionsForApiContract,
		private readonly values: GlobalValues,
		private readonly artistRepo: ArtistRepository,
		private readonly userRepo: UserRepository,
	) {
		makeObservable(this);

		this.active = contract.active;
		this.email = contract.email;
		this.groupId = contract.groupId;
		this.name = contract.name;
		this.ownedArtists = contract.ownedArtistEntries;
		this.permissions = Object.values(PermissionToken)
			.filter((value) => value !== PermissionToken.Nothing)
			.map(
				(value) =>
					new PermissionEditStore(
						contract.additionalPermissions.includes(value),
						contract.effectivePermissions.includes(value),
						value,
					),
			);
		this.poisoned = contract.poisoned;
		this.supporter = contract.supporter;
	}

	addArtist = async (artistId?: number): Promise<void> => {
		if (!artistId) return;

		const artist = await this.artistRepo.getOne({
			id: artistId,
			lang: this.values.languagePreference,
		});

		runInAction(() => {
			this.ownedArtists.push({ artist: artist });
		});
	};

	@action removeArtist = (ownedArtist: ArtistForUserForApiContract): void => {
		pull(this.ownedArtists, ownedArtist);
	};

	@action submit = async (requestToken: string): Promise<number> => {
		try {
			this.submitting = true;

			const id = await this.userRepo.edit(requestToken, {
				active: this.active,
				additionalPermissions: this.permissions
					.filter((permission) => permission.hasFlag)
					.map((permission) => permission.permissionToken),
				email: this.email,
				groupId: this.groupId,
				id: this.contract.id,
				name: this.name,
				ownedArtistEntries: this.ownedArtists,
				poisoned: this.poisoned,
				supporter: this.supporter,
			});

			return id;
		} catch (error: any) {
			if (error.response) {
				runInAction(() => {
					this.errors = undefined;

					if (error.response.status === 400)
						this.errors = error.response.data.errors;
				});
			}

			throw error;
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
