import { AlbumContract } from '@/types/DataContracts/Album/AlbumContract';
import { UserApiContract } from '@/types/DataContracts/User/UserApiContract';

export enum MediaType {
	PhysicalDisc = 'PhysicalDisc',
	DigitalDownload = 'DigitalDownload',
	Other = 'Other',
}

export enum PurchaseStatus {
	Nothing = 'Nothing',
	Wishlisted = 'Wishlisted',
	Ordered = 'Ordered',
	Owned = 'Owned',
}

export interface AlbumForUserForApiContract {
	album: AlbumContract;

	mediaType: MediaType;

	purchaseStatus: PurchaseStatus;

	rating: number;

	user?: UserApiContract;
}
