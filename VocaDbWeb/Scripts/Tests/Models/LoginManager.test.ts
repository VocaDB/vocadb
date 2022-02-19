import UserWithPermissionsContract from '@DataContracts/User/UserWithPermissionsContract';
import LoginManager, { PermissionToken } from '@Models/LoginManager';
import PVService from '@Models/PVs/PVService';
import UserGroup from '@Models/Users/UserGroup';
import GlobalValues from '@Shared/GlobalValues';
import _ from 'lodash';

const createUser = ({
	id = 39,
	name = 'Miku',
	active = true,
	effectivePermissions = [],
	groupId = UserGroup.Regular,
}: {
	id?: number;
	name?: string;
	active?: boolean;
	effectivePermissions?: PermissionToken[];
	groupId?: UserGroup;
}): UserWithPermissionsContract => {
	return {
		id: id,
		name: name,
		active: active,
		effectivePermissions: effectivePermissions,
		unreadMessagesCount: 0,
		verifiedArtist: false,
		ownedArtistEntries: [],
		preferredVideoService: PVService[PVService.NicoNicoDouga],
		albumFormatString: '',
		groupId: groupId,
	};
};

const createValues = (
	loggedUser?: UserWithPermissionsContract,
	lockdownMessage?: string,
): GlobalValues => {
	return {
		...vdb.values,
		isLoggedIn: !!loggedUser,
		loggedUser,
		lockdownMessage,
	};
};

describe('hasPermission', () => {
	const effectivePermissions = [
		PermissionToken.EditProfile,
		PermissionToken.CreateComments,
		PermissionToken.ManageDatabase,
		PermissionToken.EditTags,
		PermissionToken.ReportUser,
		PermissionToken.ManageEventSeries,
	];

	const testHasPermission = (
		loginManager: LoginManager,
		tokens: PermissionToken[],
	): void => {
		_.forEach(tokens, (token) => {
			const result = loginManager.hasPermission(token);
			expect(result).toBe(true);
		});

		_.xor(Object.values(PermissionToken), tokens).forEach((token) => {
			const result = loginManager.hasPermission(token);
			expect(result).toBe(false);
		});
	};

	test('not logged in', () => {
		const values = createValues();
		const loginManager = new LoginManager(values);

		testHasPermission(loginManager, [PermissionToken.Nothing]);
	});

	test('disabled user', () => {
		const user = createUser({
			active: false,
			effectivePermissions,
		});
		const values = createValues(user);
		const loginManager = new LoginManager(values);

		testHasPermission(loginManager, [PermissionToken.Nothing]);
	});

	test('regular user', () => {
		const user = createUser({
			effectivePermissions,
		});
		const values = createValues(user);
		const loginManager = new LoginManager(values);

		testHasPermission(loginManager, [
			PermissionToken.Nothing,
			...effectivePermissions,
		]);
	});

	test('lockdown message', () => {
		const user = createUser({
			effectivePermissions,
		});
		const values = createValues(user, 'lockdown message');
		const loginManager = new LoginManager(values);

		testHasPermission(loginManager, [
			PermissionToken.Nothing,
			..._.difference(effectivePermissions, [PermissionToken.ManageDatabase]),
		]);
	});
});
