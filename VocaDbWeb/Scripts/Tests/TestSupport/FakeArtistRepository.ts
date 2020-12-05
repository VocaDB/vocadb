import ArtistContract from '../../DataContracts/Artist/ArtistContract';
import ArtistRepository from '../../Repositories/ArtistRepository';

    export default class FakeArtistRepository extends ArtistRepository {

        result: ArtistContract = null;

        constructor() {

            super("");

            this.getOne = (id, callback: (result: ArtistContract) => void) => {
                if (callback)
                    callback(this.result);
            };

        }

    }