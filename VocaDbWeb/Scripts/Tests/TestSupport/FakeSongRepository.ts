import { NewSongCheckResultContract } from '@/DataContracts/NewSongCheckResultContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { SongListBaseContract } from '@/DataContracts/SongListBaseContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import {
	SongOptionalField,
	SongRepository,
} from '@/Repositories/SongRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { FakePromise } from '@/Tests/TestSupport/FakePromise';
import { max } from 'lodash-es';

export interface SongInList {
	listId: number;
	songId: number;
	notes: string;
}

export class FakeSongRepository extends SongRepository {
	public results: NewSongCheckResultContract = null!;
	public song: SongApiContract = null!;
	public songLists: SongListBaseContract[] = [];
	public songsInLists: SongInList[] = [];

	public constructor() {
		super(new HttpClient(), '');

		this.addSongToList = ({
			listId,
			songId,
			notes,
			newListName,
		}: {
			listId: number;
			songId: number;
			notes: string;
			newListName: string;
		}): Promise<void> => {
			if (listId !== 0) {
				this.songsInLists.push({
					listId: listId,
					songId: songId,
					notes: notes,
				});
			} else {
				const nextListId = (max(this.songLists.map((sl) => sl.id)) || 0) + 1;
				this.songLists.push({ id: nextListId, name: newListName });
				this.songsInLists.push({
					listId: nextListId,
					songId: songId,
					notes: notes,
				});
			}

			return Promise.resolve();
		};

		this.findDuplicate = ({
			params,
		}: {
			params: {
				term: string[];
				pv: string[];
				artistIds: number[];
				getPVInfo: boolean;
			};
		}): Promise<NewSongCheckResultContract> => {
			return FakePromise.resolve(this.results);
		};

		this.getOneWithComponents = ({
			id,
			fields,
			lang,
		}: {
			id: number;
			fields?: SongOptionalField[];
			lang: ContentLanguagePreference;
		}): Promise<SongApiContract> => {
			return FakePromise.resolve(this.song);
		};

		this.songListsForSong = ({
			songId,
		}: {
			songId: number;
		}): Promise<string> => {
			return FakePromise.resolve('Miku!');
		};

		this.songListsForUser = ({
			ignoreSongId,
		}: {
			ignoreSongId: number;
		}): Promise<SongListBaseContract[]> => {
			return FakePromise.resolve(this.songLists);
		};
	}
}
