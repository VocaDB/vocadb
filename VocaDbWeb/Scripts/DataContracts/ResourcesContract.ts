
//module vdb.dataContracts {
	
	// From ResourcesApiController
	export default interface ResourcesContract {

		activityEntry_activityFeedEventNames?: { [key: string]: string; }
		album_albumEditableFieldNames?: { [key: string]: string; }
		artist_artistEditableFieldNames?: { [key: string]: string; }
		releaseEvent_releaseEventEditableFieldNames?: { [key: string]: string; }
		song_songEditableFieldNames?: { [key: string]: string; }
		songList_songListEditableFieldNames?: { [key: string]: string; }
		songList_songListFeaturedCategoryNames?: { [key: string]: string; }
		tag_tagEditableFieldNames?: { [key: string]: string; }
		user_ratedSongForUserSortRuleNames?: { [key: string]: string; }

		albumCollectionStatusNames?: { [key: string]: string; }
		albumMediaTypeNames?: { [key: string]: string; }
		albumSortRuleNames?: { [key: string]: string; }
		artistSortRuleNames?: { [key: string]: string; }
		artistTypeNames?: { [key: string]: string; }
		discTypeNames?: { [key: string]: string; }
		entryTypeNames?: { [key: string]: string; }
		eventCategoryNames?: { [key: string]: string; }
		eventSortRuleNames?: { [key: string]: string; }
		songListSortRuleNames?: { [key: string]: string; }
		songSortRuleNames?: { [key: string]: string; }
		songTypeNames?: { [key: string]: string; }
		userGroupNames?: { [key: string]: string; }

	}

//}