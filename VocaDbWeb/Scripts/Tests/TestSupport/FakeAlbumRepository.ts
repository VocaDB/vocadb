import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { HttpClient } from '@/Shared/HttpClient';

export class FakeAlbumRepository extends AlbumRepository {
	deletedId!: number;
	updatedId!: number;

	constructor() {
		super(new HttpClient(), '');
	}
}
