import { GlobalValues } from '@/types/GlobalValues';
import { PermissionToken } from '@/types/Models/LoginManager';

const ALWAYS_PERMISSIONS = [
	PermissionToken.ViewLyrics,
	PermissionToken.ViewCoverArtImages,
	PermissionToken.ViewOtherPVs,
];

export const hasPermission = (
	values: GlobalValues | undefined,
	token: PermissionToken
): boolean => {
	if (token === PermissionToken.Nothing) return true;
	if (values === undefined) return false;

	if (values.alwaysPermissions) {
		if (ALWAYS_PERMISSIONS.includes(token)) {
			return true;
		}
	}

	if (!values.loggedUser || !values.loggedUser.active) return false;

	if (token === PermissionToken.ManageDatabase && !!values.lockdownMessage) return false;

	return values.loggedUser.effectivePermissions.includes(token);
};

