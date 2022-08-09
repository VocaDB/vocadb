import { EntryType } from '@/Models/EntryType';

export enum TagTargetTypes {
	Nothing = EntryType.Undefined,
	Album = EntryType.Album,
	Artist = EntryType.Artist,
	Song = EntryType.Song,
	Event = EntryType.ReleaseEvent,
	SongList = EntryType.SongList,
	All = 1073741823,
}
