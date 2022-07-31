import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { ArtistLinkType } from '@/Models/Artists/ArtistLinkType';

export interface ArchivedArtistForArtistContract extends ObjectRefContract {
	linkType: ArtistLinkType;
}
