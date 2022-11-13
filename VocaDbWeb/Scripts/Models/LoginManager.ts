import { CommentContract } from '@/DataContracts/CommentContract';
import { EntryRefContract } from '@/DataContracts/EntryRefContract';
import { SongListContract } from '@/DataContracts/Song/SongListContract';
import { UserWithPermissionsContract } from '@/DataContracts/User/UserWithPermissionsContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { IEntryWithStatus } from '@/Models/IEntryWithStatus';
import { UserGroup } from '@/Models/Users/UserGroup';
import { GlobalValues } from '@/Shared/GlobalValues';

// Corresponds to the PermissionToken struct in C#.
export enum PermissionToken {
	Nothing = '00000000-0000-0000-0000-000000000000',
	AccessManageMenu = 'b54de61d-9341-4435-8cb1-31e5e295d577',
	AddRawFileMedia = '9c1992d8-3fca-4008-b248-7d7f2a7f15dc',
	Admin = '1c98077f-f36f-4ef2-8cf3-cd9e347d389a',
	ApplyAnyTag = '100e81ce-6bc9-4083-bff4-2a47fcfb37d3',
	ApproveEntries = 'e3b4b909-5128-4a0e-9f26-2bf1d5e497ab',
	BulkDeletePVs = 'caa8f4d7-322e-44f7-ad79-7de767ef1128',
	CreateComments = 'be2deee9-ee12-48b4-a9a5-e369915fc156',
	DeleteComments = '1b1dfcfa-6b96-4a8a-8aca-d76465439ffb',
	DeleteEntries = 'cc51c6b6-be93-4942-a6e4-fdf88f4520b9',
	DesignatedStaff = 'b995a14b-49b4-4f1e-8fac-36a34967ddb0',
	DisableUsers = 'cb46dfbe-5221-4af4-9968-53aec5faa3d4',
	EditAllSongLists = 'b4873d98-b21f-40ee-b1d4-94102ae6e528',
	EditFeaturedLists = 'a639e4a3-86fe-429a-81ea-d0aa05161e40',
	EditProfile = '4f79b01a-7154-4a7f-bc87-a8a9259a9905',
	LockEntries = 'eb02e92e-207f-4330-a763-6bafd2cedde1',
	ManageDatabase = 'd762d720-79ef-4e60-8397-1d638c26d82b',
	EditTags = '2ab6da19-bc5a-4a28-86d4-8ddce399ca0b',
	ManageEntryReports = 'f9eb1d22-9142-4a04-9238-f4ebe5f1fc17',
	ManageEventSeries = 'cf39509b-b9c5-4efc-9b13-2743ffec9aac',
	ManageIPRules = 'f125fe5b-6474-4d52-823f-955c7d19f7c8',
	ManageUserPermissions = 'c0eb147e-10f5-4fea-9b19-b412ef613479',
	ManageTagMappings = 'c93667a9-a270-486d-ab00-db3ff390dd83',
	MergeEntries = 'eb336a5b-8455-4048-bc3a-8003dc522dc5',
	MikuDbImport = '0b879c57-5eba-462a-b842-d9f7dd0befd8',
	MoveToTrash = '99c333a2-ea0a-4a7b-91cb-ceef6f667389',
	ReportUser = '85c730a1-402c-4922-b617-6c6068f5f98e',
	RemoveEditPermission = '6b258b3e-97a1-4cf7-9c4f-80787c2d2266',
	RemoveTagUsages = '135aaf49-08d5-42bb-b8ed-ef1ceb910a69',
	RestoreRevisions = 'e99a1e1c-1742-48c1-877b-17cb2964e8bc',
	UploadMedia = 'be1a2f04-6dc2-4d63-b34e-4499f6389231',
	ViewAuditLog = '8d3d5395-12c9-440a-8120-4911034b9a7e',
	ViewHiddenRatings = '47bcc523-5667-403d-bd20-d2728e1f9c5f',
	ViewHiddenRevisions = 'c3b753d0-7aa8-4c03-8bca-5311fb2bdd2d',
	ManageWebhooks = '838dde1d-51ba-423b-ad8e-c1e2c2024a37',
	CreateDatabaseDump = 'd3dffb90-2408-4434-ae3a-c26352293281',
}

// Corresponds to the LoginManager and EntryPermissionManager classes in C#.
export class LoginManager {
	constructor(private readonly values: GlobalValues) {}

	get isLoggedIn(): boolean {
		return this.values.isLoggedIn;
	}

	get loggedUserId(): number {
		return this.values.loggedUserId;
	}

	get loggedUser(): UserWithPermissionsContract | undefined {
		return this.values.loggedUser;
	}

	get lockdownEnabled(): boolean {
		return !!this.values.lockdownMessage;
	}

	hasPermission = (token: PermissionToken): boolean => {
		if (token === PermissionToken.Nothing) return true;

		if (!this.loggedUser || !this.loggedUser.active) return false;

		if (token === PermissionToken.ManageDatabase && this.lockdownEnabled)
			return false;

		return this.loggedUser.effectivePermissions.includes(token);
	};

	get canAccessManageMenu(): boolean {
		return this.hasPermission(PermissionToken.AccessManageMenu);
	}

	get canAdmin(): boolean {
		return this.hasPermission(PermissionToken.Admin);
	}

	get canApproveEntries(): boolean {
		return this.hasPermission(PermissionToken.ApproveEntries);
	}

	get canBulkDeletePVs(): boolean {
		return this.hasPermission(PermissionToken.BulkDeletePVs);
	}

	get canCreateComments(): boolean {
		return this.hasPermission(PermissionToken.CreateComments);
	}

	get canDeleteComments(): boolean {
		return this.hasPermission(PermissionToken.DeleteComments);
	}

	get canDeleteEntries(): boolean {
		return this.hasPermission(PermissionToken.DeleteEntries);
	}

	get canDisableUsers(): boolean {
		return this.hasPermission(PermissionToken.DisableUsers);
	}

	get canEditAllSongLists(): boolean {
		return this.hasPermission(PermissionToken.EditAllSongLists);
	}

	get canEditFeaturedLists(): boolean {
		return this.hasPermission(PermissionToken.EditFeaturedLists);
	}

	get canEditProfile(): boolean {
		return this.hasPermission(PermissionToken.EditProfile);
	}

	get canLockEntries(): boolean {
		return this.hasPermission(PermissionToken.LockEntries);
	}

	get canManageDatabase(): boolean {
		return this.hasPermission(PermissionToken.ManageDatabase);
	}

	get canEditTags(): boolean {
		return this.hasPermission(PermissionToken.EditTags);
	}

	get canManageUserPermissions(): boolean {
		return this.hasPermission(PermissionToken.ManageUserPermissions);
	}

	get canManageEntryReports(): boolean {
		return this.hasPermission(PermissionToken.ManageEntryReports);
	}

	get canManageIPRules(): boolean {
		return this.hasPermission(PermissionToken.ManageIPRules);
	}

	get canManageTagMappings(): boolean {
		return this.hasPermission(PermissionToken.ManageTagMappings);
	}

	get canMergeEntries(): boolean {
		return this.hasPermission(PermissionToken.MergeEntries);
	}

	get canMikuDbImport(): boolean {
		return this.hasPermission(PermissionToken.MikuDbImport);
	}

	get canMoveToTrash(): boolean {
		return this.hasPermission(PermissionToken.MoveToTrash);
	}

	get canRemoveEditPermission(): boolean {
		return this.hasPermission(PermissionToken.RemoveEditPermission);
	}

	get canReportUser(): boolean {
		return this.hasPermission(PermissionToken.ReportUser);
	}

	get canRestoreRevisions(): boolean {
		return this.hasPermission(PermissionToken.RestoreRevisions);
	}

	get canUploadMedia(): boolean {
		return this.hasPermission(PermissionToken.UploadMedia);
	}

	get canViewAuditLog(): boolean {
		return this.hasPermission(PermissionToken.ViewAuditLog);
	}

	get canViewHiddenRevisions(): boolean {
		return this.hasPermission(PermissionToken.ViewHiddenRevisions);
	}

	get canManageWebhooks(): boolean {
		return this.hasPermission(PermissionToken.ManageWebhooks);
	}

	private static readonly allPermissions: EntryStatus[] = [
		EntryStatus.Draft,
		EntryStatus.Finished,
		EntryStatus.Approved,
		EntryStatus.Locked,
	] /* TODO: use Object.values */;

	// Entry statuses allowed for normal users
	private static readonly normalStatusPermissions = [
		EntryStatus.Draft,
		EntryStatus.Finished,
	];

	// Entry statuses allowed for trusted users
	private static readonly trustedStatusPermissions = [
		EntryStatus.Draft,
		EntryStatus.Finished,
		EntryStatus.Approved,
	];

	canDeleteComment = (comment: CommentContract): boolean => {
		return (
			this.canDeleteComments ||
			(!!comment.author &&
				!!this.loggedUser &&
				comment.author.id === this.loggedUser.id)
		);
	};

	private isDirectlyVerifiedFor = (entry?: EntryRefContract): boolean => {
		return (
			!!entry &&
			entry.entryType === EntryType[EntryType.Artist] &&
			!!this.loggedUser &&
			this.loggedUser.verifiedArtist &&
			this.loggedUser.ownedArtistEntries.some((a) => a.artist.id === entry.id)
		);
	};

	/// <summary>
	/// Gets a list of entry statuses that the user can edit or set.
	/// This means, the user is allowed to edit entries with any of these statuses,
	/// and the user is able to change the entry status to any of these.
	/// </summary>
	/// <remarks>
	/// Most of the time the allowed entry statuses are global, but associating a user account with an artist entry
	/// gives special entry-specific permissions for the user editing that entry.
	/// </remarks>
	/// <param name="permissionContext">User permission context identifying the user's global permissions.</param>
	/// <param name="entry">Entry to be checked. Can be null. If null, only global permissions will be checked.</param>
	/// <returns>A list of permissions that can be set by the user.</returns>
	allowedEntryStatuses = (entry?: EntryRefContract): EntryStatus[] => {
		// Check for basic edit permissions, without these the user is limited or disabled
		if (!this.canManageDatabase) return [];

		// Moderators with lock permissions can edit everything
		if (this.canLockEntries) return LoginManager.allPermissions;

		// Trusted users can edit approved entries
		if (this.canApproveEntries) return LoginManager.trustedStatusPermissions;

		// Verified artists get trusted permissions for their own entry
		if (this.isDirectlyVerifiedFor(entry))
			return LoginManager.trustedStatusPermissions;

		// Normal user permissions
		if (this.canManageDatabase) return LoginManager.normalStatusPermissions;

		return [];
	};

	/// <summary>
	/// Tests whether the user can edit a specific entry.
	/// The permission depends on both the user's global permissions and entry status.
	/// </summary>
	/// <param name="permissionContext">User permission context. Cannot be null.</param>
	/// <param name="entry">Entry to be checked. Cannot be null.</param>
	/// <returns>True if the user can edit the entry, otherwise false.</returns>
	canEdit = (entry: IEntryWithStatus): boolean => {
		return this.allowedEntryStatuses(entry).includes(entry.status);
	};

	canEditSongList = (songList: SongListContract): boolean => {
		if (songList.featuredCategory !== 'Nothing' && this.canEditFeaturedLists)
			return true;

		if (this.canEditAllSongLists) return true;

		return songList.author.id === this.loggedUserId;
	};

	canEditGroupTo = (groupId: UserGroup): boolean => {
		return (
			!!this.loggedUser &&
			(this.loggedUser.groupId === UserGroup.Admin ||
				Object.values(UserGroup).indexOf(this.loggedUser.groupId) >
					Object.values(UserGroup).indexOf(groupId))
		);
	};

	canEditUser = (groupId: UserGroup): boolean => {
		return this.canEditGroupTo(groupId);
	};

	canEditTagsForEntry = (entry: IEntryWithStatus): boolean => {
		return this.canEditTags && this.canEdit(entry);
	};
}

export const loginManager = new LoginManager(vdb.values);
