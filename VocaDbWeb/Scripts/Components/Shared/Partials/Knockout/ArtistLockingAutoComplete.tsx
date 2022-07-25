import ArtistContract from '@DataContracts/Artist/ArtistContract';
import ArtistType from '@Models/Artists/ArtistType';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import ArtistAutoComplete from '../../../KnockoutExtensions/ArtistAutoComplete';
import LockingAutoComplete from './LockingAutoComplete';

interface ArtistLockingAutoCompleteProps {
	basicEntryLinkStore: BasicEntryLinkStore<ArtistContract>;
	artistTypes?: ArtistType[];
}

// Locking autocomplete for artist selection. Allows selection of one (existing) artist. When artist is selected, clear button is displayed.
const ArtistLockingAutoComplete = observer(
	({
		basicEntryLinkStore,
		artistTypes,
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
						acceptSelection: (id): void =>
							runInAction(() => {
								basicEntryLinkStore.id = id;
							}),
					}}
					placeholder={t('ViewRes:Shared.Search')}
					artistTypes={artistTypes}
				/>
			</LockingAutoComplete>
		);
	},
);

export default ArtistLockingAutoComplete;
