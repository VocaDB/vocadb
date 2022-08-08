import IEntryWithIdAndName from '@/Models/IEntryWithIdAndName';
import BasicEntryLinkStore from '@/Stores/BasicEntryLinkStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import ReleaseEventSeriesAutoComplete from '../../../KnockoutExtensions/ReleaseEventSeriesAutoComplete';
import LockingAutoComplete from './LockingAutoComplete';

interface ReleaseEventSeriesLockingAutoCompleteProps {
	basicEntryLinkStore: BasicEntryLinkStore<IEntryWithIdAndName>;
}

const ReleaseEventSeriesLockingAutoComplete = observer(
	({
		basicEntryLinkStore,
	}: ReleaseEventSeriesLockingAutoCompleteProps): React.ReactElement => {
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
				<ReleaseEventSeriesAutoComplete
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

export default ReleaseEventSeriesLockingAutoComplete;
