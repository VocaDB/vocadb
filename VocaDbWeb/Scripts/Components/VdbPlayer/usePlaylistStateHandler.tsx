import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import {
	PlayQueueItem,
	PlayQueueItemContract,
} from '@/Stores/VdbPlayer/PlayQueueStore';
import { reaction, runInAction } from 'mobx';
import React from 'react';

const itemsKey = 'playlist.items';
const currentIndexKey = 'playlist.currentIndex';
const totalCountKey = 'playlist.totalCount';
const pageKey = 'playlist.page';

export const usePlaylistStateHandler = (): void => {
	const { playQueue } = useVdbPlayer();

	React.useEffect(() => {
		try {
			const serializedItemContracts = window.localStorage.getItem(itemsKey);

			if (serializedItemContracts) {
				const itemContracts = JSON.parse(
					serializedItemContracts,
				) as PlayQueueItemContract[];

				runInAction(() => {
					playQueue.items = itemContracts.map(PlayQueueItem.fromContract);
					playQueue.currentIndex = Number(
						window.localStorage.getItem(currentIndexKey),
					);

					playQueue.totalCount =
						Number(window.localStorage.getItem(totalCountKey)) || 0;
					playQueue.page = Number(window.localStorage.getItem(pageKey)) || 1;
				});
			}
		} catch (error) {
			/* ignore */
		}
	}, [playQueue]);

	React.useEffect(() => {
		return reaction(
			() => playQueue.items.map((item) => item),
			(items) => {
				window.localStorage.setItem(
					itemsKey,
					JSON.stringify(items.map((item) => item.toContract())),
				);
			},
		);
	}, [playQueue]);

	React.useEffect(() => {
		return reaction(
			() => playQueue.currentIndex,
			(currentIndex) => {
				window.localStorage.setItem(
					currentIndexKey,
					JSON.stringify(currentIndex),
				);
			},
		);
	}, [playQueue]);

	React.useEffect(() => {
		return reaction(
			() => playQueue.totalCount,
			(totalItems) => {
				window.localStorage.setItem(totalCountKey, JSON.stringify(totalItems));
			},
		);
	}, [playQueue]);

	React.useEffect(() => {
		return reaction(
			() => playQueue.page,
			(page) => {
				window.localStorage.setItem(pageKey, JSON.stringify(page));
			},
		);
	}, [playQueue]);
};
