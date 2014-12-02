/// <reference path="../../typings/knockout/knockout.d.ts" />

module vdb.viewModels {

    export class AlbumCreateViewModel {

        public submit = () => {
            this.submitting(true);
            return true;
        }

        public submitting = ko.observable(false);

    }

}