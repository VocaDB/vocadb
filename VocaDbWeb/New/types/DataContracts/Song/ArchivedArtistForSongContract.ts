import { ObjectRefContract } from '@/types/DataContracts/ObjectRefContract';
import { ArtistRoles } from '@/types/Models/Artists/ArtistRoles';

export interface ArchivedArtistForSongContract extends ObjectRefContract {
	isSupport: boolean;
	roles: ArtistRoles;
}
