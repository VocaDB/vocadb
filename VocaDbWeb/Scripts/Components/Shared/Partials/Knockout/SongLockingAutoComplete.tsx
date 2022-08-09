import { SongAutoComplete } from '@/Components/KnockoutExtensions/SongAutoComplete';
import { LockingAutoComplete } from '@/Components/Shared/Partials/Knockout/LockingAutoComplete';
import { SongContract } from '@/DataContracts/Song/SongContract';
import { EntryType } from '@/Models/EntryType';
import { SongType } from '@/Models/Songs/SongType';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SongLockingAutoCompleteProps {
	basicEntryLinkStore: BasicEntryLinkStore<SongContract>;
	songTypes?: SongType[];
	ignoreId?: number;
	height?: number;
}

// Locking autocomplete for song selection. Allows selection of one (existing) song. When song is selected, clear button is displayed.
export const SongLockingAutoComplete = observer(
	({
		basicEntryLinkStore,
		songTypes = [],
		ignoreId,
		height,
	}: SongLockingAutoCompleteProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<LockingAutoComplete
				text={basicEntryLinkStore.name}
				value={basicEntryLinkStore.id}
				entryType={EntryType.Song}
				onClear={(): void =>
					runInAction(() => {
						basicEntryLinkStore.id = undefined;
					})
				}
			>
				<SongAutoComplete
					type="text"
					className="input-large"
					properties={{
						acceptSelection: (id): void =>
							runInAction(() => {
								basicEntryLinkStore.id = id;
							}),
						extraQueryParams: { songTypes: songTypes },
						ignoreId: ignoreId,
						height: height,
					}}
					placeholder={t('ViewRes:Shared.Search')}
				/>
			</LockingAutoComplete>
		);
	},
);
