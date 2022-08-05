import { ArtistForUserForApiContract } from '@/DataContracts/User/ArtistForUserForApiContract';
import { UserWithPermissionsForApiContract } from '@/DataContracts/User/UserWithPermissionsForApiContract';
import { PermissionToken } from '@/Models/LoginManager';
import { UserGroup } from '@/Models/Users/UserGroup';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import _ from 'lodash';
import { action, makeObservable, observable, runInAction } from 'mobx';

export class PermissionEditStore {
	@observable public hasFlag: boolean;

	public constructor(
		hasFlag: boolean,
		public readonly hasPermission: boolean,
		public readonly permissionToken: PermissionToken,
	) {
		makeObservable(this);

		this.hasFlag = hasFlag;
	}
}

export class UserEditStore {
	@observable public active: boolean;
	@observable public email: string;
	@observable public errors?: Record<string, string[]>;
	@observable public groupId: UserGroup;
	@observable public name: string;
	@observable public ownedArtists: ArtistForUserForApiContract[];
	@observable public permissions: PermissionEditStore[];
	@observable public poisoned: boolean;
	@observable public submitting = false;
	@observable public supporter: boolean;

	public constructor(
		public readonly contract: UserWithPermissionsForApiContract,
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

	public addArtist = async (artistId?: number): Promise<void> => {
		if (!artistId) return;

		const artist = await this.artistRepo.getOne({
			id: artistId,
			lang: this.values.languagePreference,
		});

		runInAction(() => {
			this.ownedArtists.push({ artist: artist });
		});
	};

	@action public removeArtist = (
		ownedArtist: ArtistForUserForApiContract,
	): void => {
		_.pull(this.ownedArtists, ownedArtist);
	};

	@action public submit = async (requestToken: string): Promise<number> => {
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
