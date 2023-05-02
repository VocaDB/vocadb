import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { PVService } from '@/Models/PVs/PVService';
import { UserGroup } from '@/Models/Users/UserGroup';

// Corresponds to the ArtistForUserContract class in C#.
interface ArtistForUserContract {
	artist: ArtistContract;
}

// Corresponds to the SanitizedUserWithPermissionsContract record class in C#.
export interface UserWithPermissionsContract {
	id: number;
	name: string;
	active: boolean;
	effectivePermissions: string[];
	unreadMessagesCount: number;
	verifiedArtist: boolean;
	ownedArtistEntries: ArtistForUserContract[];
	preferredVideoService: PVService;
	albumFormatString: string;
	groupId: UserGroup;
	stylesheet: string
}
