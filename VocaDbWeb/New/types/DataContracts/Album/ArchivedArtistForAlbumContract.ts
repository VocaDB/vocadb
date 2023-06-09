import { ObjectRefContract } from '@/types/DataContracts/ObjectRefContract';
import { ArtistRoles } from '@/types/Models/Artists/ArtistRoles';

export interface ArchivedArtistForAlbumContract extends ObjectRefContract {
	isSupport: boolean;
	roles: ArtistRoles;
}
