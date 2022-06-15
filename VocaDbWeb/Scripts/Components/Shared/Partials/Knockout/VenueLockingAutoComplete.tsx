import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import VenueAutoComplete from '../../../KnockoutExtensions/VenueAutoComplete';
import LockingAutoComplete from './LockingAutoComplete';

interface VenueLockingAutoCompleteProps {
	basicEntryLinkStore: BasicEntryLinkStore<IEntryWithIdAndName>;
}

const VenueLockingAutoComplete = observer(
	({
		basicEntryLinkStore,
	}: VenueLockingAutoCompleteProps): React.ReactElement => {
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
				<VenueAutoComplete
					type="text"
					className="input-large"
					onAcceptSelection={(entry): void =>
						runInAction(() => {
							basicEntryLinkStore.id = entry.id;
						})
					}
					placeholder={t('ViewRes:Shared.Search')}
				/>
			</LockingAutoComplete>
		);
	},
);

export default VenueLockingAutoComplete;
