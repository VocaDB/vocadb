import { ObjectRefContract } from '@/types/DataContracts/ObjectRefContract';
import { ArtistEventRoles } from '@/types/Models/Events/ArtistEventRoles';

export interface ArchivedArtistForEventContract extends ObjectRefContract {
	roles: ArtistEventRoles;
}
