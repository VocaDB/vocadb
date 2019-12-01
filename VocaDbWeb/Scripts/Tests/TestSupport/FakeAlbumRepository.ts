/// <reference path="../../Repositories/AlbumRepository.ts" />

import AlbumRepository from '../../Repositories/AlbumRepository';

//module vdb.tests.testSupport {

    export default class FakeAlbumRepository extends AlbumRepository {

        public deletedId: number;
        public updatedId: number;

        constructor() {

            super("");

        }

    }

//}