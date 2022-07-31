import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { ArtistRoles } from '@/Models/Artists/ArtistRoles';

export interface ArchivedArtistForAlbumContract extends ObjectRefContract {
	isSupport: boolean;
	roles: ArtistRoles;
}
