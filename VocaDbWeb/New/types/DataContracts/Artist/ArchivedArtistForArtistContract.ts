import { ObjectRefContract } from '@/types/DataContracts/ObjectRefContract';
import { ArtistLinkType } from '@/types/Models/Artists/ArtistLinkType';

export interface ArchivedArtistForArtistContract extends ObjectRefContract {
	linkType: ArtistLinkType;
}
