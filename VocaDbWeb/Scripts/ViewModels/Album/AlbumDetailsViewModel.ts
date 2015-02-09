/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../Shared/GlobalFunctions.ts" />

module vdb.viewModels {

	import cls = vdb.models;
	import rep = repositories;

    export class AlbumDetailsViewModel {

		public comments: EditableCommentsViewModel;

        public downloadTagsDialog: DownloadTagsViewModel;

		public showTranslatedDescription: KnockoutObservable<boolean>;

        public usersContent = ko.observable<string>();

        public getUsers = () => {

            $.post(vdb.functions.mapAbsoluteUrl("/Album/UsersWithAlbumInCollection"), { albumId: this.id }, result => {

                this.usersContent(result);
                $("#userCollectionsPopup").dialog("open");

            });

            return false;

        };

        constructor(
			repo: rep.AlbumRepository,
			private id: number,
			loggedUserId: number,
			canDeleteAllComments: boolean,
			formatString: string,
			showTranslatedDescription: boolean) {

            this.downloadTagsDialog = new DownloadTagsViewModel(id, formatString);
			this.showTranslatedDescription = ko.observable(showTranslatedDescription);
			this.comments = new EditableCommentsViewModel(repo, id, loggedUserId, canDeleteAllComments, false);

        }

    }

    export class DownloadTagsViewModel {

        public dialogVisible = ko.observable(false);

        public downloadTags = () => {

            this.dialogVisible(false);

            var url = "/Album/DownloadTags/" + this.albumId;
            window.location.href = url + "?setFormatString=true&formatString=" + encodeURIComponent(this.formatString());

        };

        public formatString: KnockoutObservable<string>;

        public dialogButtons = ko.observableArray([
            { text: vdb.resources.albumDetails.download, click: this.downloadTags },
        ]);

        public show = () => {

            this.dialogVisible(true);

        };

        constructor(private albumId: number, formatString: string) {
            this.formatString = ko.observable(formatString)
        }

    }

}