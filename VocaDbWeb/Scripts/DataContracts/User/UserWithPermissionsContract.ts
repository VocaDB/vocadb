import ArtistContract from '@DataContracts/Artist/ArtistContract';

// Corresponds to the ArtistForUserContract class in C#.
interface ArtistForUserContract {
	artist: ArtistContract;
}

// Corresponds to the SanitizedUserWithPermissionsContract record class in C#.
export default interface UserWithPermissionsContract {
	id: number;
	name: string;
	active: boolean;
	effectivePermissions: string[];
	unreadMessagesCount: number;
	verifiedArtist: boolean;
	ownedArtistEntries: ArtistForUserContract[];
	preferredVideoService: string /* TODO: enum */;
}
