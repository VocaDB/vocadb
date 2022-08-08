import UserBaseContract from '@/DataContracts/User/UserBaseContract';
import BasicEntryLinkStore from '@/Stores/BasicEntryLinkStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import UserAutoComplete from '../../../KnockoutExtensions/UserAutoComplete';
import LockingAutoComplete from './LockingAutoComplete';

interface UserLockingAutoCompleteProps {
	basicEntryLinkStore: BasicEntryLinkStore<UserBaseContract>;
}

// Locking autocomplete for tag selection. Allows selection of one (existing) tag. When tag is selected, clear button is displayed.
const UserLockingAutoComplete = observer(
	({
		basicEntryLinkStore,
	}: UserLockingAutoCompleteProps): React.ReactElement => {
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
				<UserAutoComplete
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

export default UserLockingAutoComplete;
