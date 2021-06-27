export default interface UserWithPermissionsContract {
	id: number;
	name: string;
	active: boolean;
	effectivePermissions: string[];
}
