import { AlbumAutoComplete } from '@/Components/KnockoutExtensions/AlbumAutoComplete';
import { AlbumAutoCompleteParams } from '@/Components/KnockoutExtensions/AutoCompleteParams';
import { LockingAutoComplete } from '@/Components/Shared/Partials/Knockout/LockingAutoComplete';
import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface AlbumLockingAutoCompleteProps {
	basicEntryLinkStore: BasicEntryLinkStore<AlbumContract>;
	properties?: Omit<AlbumAutoCompleteParams, 'acceptSelection'>;
}

// Locking autocomplete for album selection. Allows selection of one (existing) album. When album is selected, clear button is displayed.
export const AlbumLockingAutoComplete = observer(
	({
		basicEntryLinkStore,
		properties,
	}: AlbumLockingAutoCompleteProps): React.ReactElement => {
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
				<AlbumAutoComplete
					type="text"
					className="input-large"
					properties={{
						...properties,
						acceptSelection: (id): void =>
							runInAction(() => {
								basicEntryLinkStore.id = id;
							}),
					}}
					placeholder={t('ViewRes:Shared.Search')}
				/>
			</LockingAutoComplete>
		);
	},
);
