import AlbumContract from '../Album/AlbumContract';
import UserApiContract from './UserApiContract';

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
