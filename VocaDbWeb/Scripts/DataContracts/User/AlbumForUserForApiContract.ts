import AlbumContract from '@/DataContracts/Album/AlbumContract';
import UserApiContract from '@/DataContracts/User/UserApiContract';

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

export default interface AlbumForUserForApiContract {
	album: AlbumContract;

	mediaType: MediaType;

	purchaseStatus: PurchaseStatus;

	rating: number;

	user?: UserApiContract;
}
