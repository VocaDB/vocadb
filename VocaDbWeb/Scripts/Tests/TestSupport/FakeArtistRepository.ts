/// <reference path="../../Repositories/ArtistRepository.ts" />

//module vdb.tests.testSupport {

    import dc = vdb.dataContracts;

    export class FakeArtistRepository extends vdb.repositories.ArtistRepository {

        result: dc.ArtistContract = null;

        constructor() {

            super("");

            this.getOne = (id, callback: (result: dc.ArtistContract) => void) => {
                if (callback)
                    callback(this.result);
            };

        }

    }

//}