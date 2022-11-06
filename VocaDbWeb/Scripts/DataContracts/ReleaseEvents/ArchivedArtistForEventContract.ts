import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { ArtistEventRoles } from '@/Models/Events/ArtistEventRoles';

export interface ArchivedArtistForEventContract extends ObjectRefContract {
	roles: ArtistEventRoles;
}
