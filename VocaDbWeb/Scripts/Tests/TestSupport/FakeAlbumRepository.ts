/// <reference path="../../Repositories/AlbumRepository.ts" />

module vdb.tests.testSupport {

    export class FakeAlbumRepository extends vdb.repositories.AlbumRepository {

        public deletedId: number;
        public updatedId: number;

        constructor() {

            super("");

        }

    }

}