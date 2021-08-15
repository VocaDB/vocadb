import ReleaseEventAutoComplete from '@Components/KnockoutExtensions/ReleaseEventAutoComplete';
import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import LockingAutoComplete from './LockingAutoComplete';

interface ReleaseEventLockingAutoCompleteProps {
	basicEntryLinkStore: BasicEntryLinkStore<IEntryWithIdAndName>;
}

const ReleaseEventLockingAutoComplete = observer(
	({
		basicEntryLinkStore,
	}: ReleaseEventLockingAutoCompleteProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<LockingAutoComplete
				text={basicEntryLinkStore.name}
				value={basicEntryLinkStore.id}
				onClear={(): void =>
					runInAction(() => {
						basicEntryLinkStore.entry = undefined;
					})
				}
			>
				<ReleaseEventAutoComplete
					type="text"
					className="input-large"
					onAcceptSelection={(entry): void =>
						runInAction(() => {
							basicEntryLinkStore.entry = entry;
						})
					}
					placeholder={t('ViewRes:Shared.Search')}
				/>
			</LockingAutoComplete>
		);
	},
);

export default ReleaseEventLockingAutoComplete;
