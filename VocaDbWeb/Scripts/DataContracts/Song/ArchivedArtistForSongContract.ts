import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { ArtistRoles } from '@/Models/Artists/ArtistRoles';

export interface ArchivedArtistForSongContract extends ObjectRefContract {
	isSupport: boolean;
	roles: ArtistRoles;
}
