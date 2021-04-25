import AlbumRepository from '../../Repositories/AlbumRepository';

export default class FakeAlbumRepository extends AlbumRepository {
  public deletedId: number;
  public updatedId: number;

  constructor() {
    super('');
  }
}
