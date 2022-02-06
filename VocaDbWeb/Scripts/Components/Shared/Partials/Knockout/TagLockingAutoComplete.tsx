import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import TagAutoComplete from '../../../KnockoutExtensions/TagAutoComplete';
import LockingAutoComplete from './LockingAutoComplete';

interface TagLockingAutoCompleteProps {
	basicEntryLinkStore: BasicEntryLinkStore<TagBaseContract>;
}

// Locking autocomplete for tag selection. Allows selection of one (existing) tag. When tag is selected, clear button is displayed.
const TagLockingAutoComplete = observer(
	({
		basicEntryLinkStore,
	}: TagLockingAutoCompleteProps): React.ReactElement => {
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
				<TagAutoComplete
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

export default TagLockingAutoComplete;
