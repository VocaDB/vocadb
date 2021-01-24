import CommentContract from "../../DataContracts/CommentContract";
import PartialFindResultContract from "../../DataContracts/PartialFindResultContract";
import EntryType from "../../Models/EntryType";
import ResourcesManager, { ResourceSetNames } from "../../Models/ResourcesManager";
import ResourceRepository from "../../Repositories/ResourceRepository";
import UrlMapper from "../../Shared/UrlMapper";

enum CommentSortRule {

	CreateDateDescending,

	CreateDate,

}

export default class CommentListViewModel {

	constructor(private urlMapper: UrlMapper,
		resourceRepo: ResourceRepository,
		cultureCode: string,
		private userId?: number) {

		this.entryType = ko.observable(EntryType[EntryType.Undefined]);
		this.sort = ko.observable(CommentSortRule[CommentSortRule.CreateDateDescending]);

		this.entryType.subscribe(this.clear);
		this.sort.subscribe(this.clear);

		this.resources = new ResourcesManager(resourceRepo, cultureCode);
		this.resources.loadResources(this.loadMore, ResourceSetNames.artistTypeNames, ResourceSetNames.discTypeNames, ResourceSetNames.songTypeNames,
			ResourceSetNames.userGroupNames, ResourceSetNames.activityEntry.activityFeedEventNames, ResourceSetNames.album.albumEditableFieldNames, ResourceSetNames.artist.artistEditableFieldNames,
			ResourceSetNames.releaseEvent.releaseEventEditableFieldNames,
			ResourceSetNames.song.songEditableFieldNames, ResourceSetNames.songList.songListEditableFieldNames, ResourceSetNames.songList.songListFeaturedCategoryNames,
			ResourceSetNames.tag.tagEditableFieldNames, "commentSortRuleNames");

		this.sortName = ko.computed(() => this.resources.resources().commentSortRuleNames != null ? this.resources.resources().commentSortRuleNames[this.sort()] : "");

	}

	private clear = () => {
		this.lastCommentDate = null;
		this.comments([]);
		this.loadMore();
	}

	public comments = ko.observableArray<CommentContract>([]);

	public entryType: KnockoutObservable<string>;

	private lastCommentDate: Date;

	public loadMore = () => {

		var url = this.urlMapper.mapRelative("/api/comments");
		var sortRule = CommentSortRule[this.sort()];
		$.getJSON(url, {
			before: sortRule === CommentSortRule.CreateDateDescending && this.lastCommentDate ? this.lastCommentDate.toISOString() : null,
			since: sortRule === CommentSortRule.CreateDate && this.lastCommentDate ? this.lastCommentDate.toISOString() : null,
			userId: this.userId,
			entryType: this.entryType(),
			sortRule: this.sort()
		}, (result: PartialFindResultContract<CommentContract>) => {

			var entries = result.items;

			if (!entries && entries.length > 0)
				return;

			ko.utils.arrayPushAll(this.comments, entries);
			this.lastCommentDate = new Date(_.last(entries).created);

		});

	}

	public resources: ResourcesManager;

	public sort: KnockoutObservable<string>;
	public sortName: KnockoutComputed<string>;

}
