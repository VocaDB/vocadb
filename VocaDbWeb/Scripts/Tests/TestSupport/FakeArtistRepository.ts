/// <reference path="../../Repositories/ArtistRepository.ts" />

import ArtistContract from '../../DataContracts/Artist/ArtistContract';
import ArtistRepository from '../../Repositories/ArtistRepository';

//module vdb.tests.testSupport {

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

//}