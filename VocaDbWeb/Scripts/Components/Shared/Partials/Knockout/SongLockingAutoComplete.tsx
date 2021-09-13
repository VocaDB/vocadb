import SongAutoComplete from '@Components/KnockoutExtensions/SongAutoComplete';
import SongContract from '@DataContracts/Song/SongContract';
import EntryType from '@Models/EntryType';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import LockingAutoComplete from './LockingAutoComplete';

interface SongLockingAutoCompleteProps {
	basicEntryLinkStore: BasicEntryLinkStore<SongContract>;
	songTypes?: string /* TODO: enum */[];
	ignoreId?: number;
}

// Locking autocomplete for song selection. Allows selection of one (existing) song. When song is selected, clear button is displayed.
const SongLockingAutoComplete = observer(
	({
		basicEntryLinkStore,
		songTypes = [],
		ignoreId,
	}: SongLockingAutoCompleteProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<LockingAutoComplete
				text={basicEntryLinkStore.name}
				value={basicEntryLinkStore.id}
				entryType={EntryType.Song}
				onClear={(): void => basicEntryLinkStore.selectEntry(undefined)}
			>
				<SongAutoComplete
					type="text"
					className="input-large"
					properties={{
						acceptSelection: (id): void => basicEntryLinkStore.selectEntry(id),
						extraQueryParams: { songTypes: songTypes.join(',') },
						ignoreId: ignoreId,
					}}
					placeholder={t('ViewRes:Shared.Search')}
				/>
			</LockingAutoComplete>
		);
	},
);

export default SongLockingAutoComplete;
