import ArtistAutoComplete from '@/Components/KnockoutExtensions/ArtistAutoComplete';
import { ArtistAutoCompleteParams } from '@/Components/KnockoutExtensions/AutoCompleteParams';
import LockingAutoComplete from '@/Components/Shared/Partials/Knockout/LockingAutoComplete';
import ArtistContract from '@/DataContracts/Artist/ArtistContract';
import BasicEntryLinkStore from '@/Stores/BasicEntryLinkStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ArtistLockingAutoCompleteProps {
	basicEntryLinkStore: BasicEntryLinkStore<ArtistContract>;
	properties?: Omit<ArtistAutoCompleteParams, 'acceptSelection'>;
}

// Locking autocomplete for artist selection. Allows selection of one (existing) artist. When artist is selected, clear button is displayed.
const ArtistLockingAutoComplete = observer(
	({
		basicEntryLinkStore,
		properties,
	}: ArtistLockingAutoCompleteProps): React.ReactElement => {
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
				<ArtistAutoComplete
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

export default ArtistLockingAutoComplete;
