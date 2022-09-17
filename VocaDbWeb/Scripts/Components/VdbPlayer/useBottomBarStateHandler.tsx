import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { reaction, runInAction } from 'mobx';
import React from 'react';

const enabledKey = 'bottomBar.enabled';

export const useBottomBarStateHandler = (): void => {
	const { vdbPlayer } = useVdbPlayer();

	React.useEffect(() => {
		runInAction(() => {
			vdbPlayer.bottomBarEnabled =
				window.localStorage.getItem(enabledKey) !== 'false';
		});
	}, [vdbPlayer]);

	React.useEffect(() => {
		return reaction(
			() => vdbPlayer.bottomBarEnabled,
			(bottomBarEnabled) => {
				window.localStorage.setItem(
					enabledKey,
					JSON.stringify(bottomBarEnabled),
				);
			},
		);
	}, [vdbPlayer]);
};
