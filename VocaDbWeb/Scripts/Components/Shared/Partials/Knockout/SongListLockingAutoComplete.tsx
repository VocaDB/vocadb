import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import { SongListFeaturedCategory } from '@Stores/SongList/FeaturedSongListsStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import SongListAutoComplete from '../../../KnockoutExtensions/SongListAutoComplete';
import LockingAutoComplete from './LockingAutoComplete';

interface SongListLockingAutoCompleteProps {
	basicEntryLinkStore: BasicEntryLinkStore<IEntryWithIdAndName>;
	songListCategory: SongListFeaturedCategory;
}

const SongListLockingAutoComplete = observer(
	({
		basicEntryLinkStore,
		songListCategory,
	}: SongListLockingAutoCompleteProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<LockingAutoComplete
				text={basicEntryLinkStore.name}
				value={basicEntryLinkStore.id}
				onClear={(): void =>
					runInAction(() => {
						basicEntryLinkStore.id = undefined;
					})
				}
			>
				<SongListAutoComplete
					type="text"
					className="input-large"
					onAcceptSelection={(entry): void =>
						runInAction(() => {
							basicEntryLinkStore.id = entry.id;
						})
					}
					placeholder={t('ViewRes:Shared.Search')}
					songListCategory={songListCategory}
				/>
			</LockingAutoComplete>
		);
	},
);

export default SongListLockingAutoComplete;
