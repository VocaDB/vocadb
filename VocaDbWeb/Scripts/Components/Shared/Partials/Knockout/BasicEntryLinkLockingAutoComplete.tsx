import Button from '@Bootstrap/Button';
import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface BasicEntryLinkLockingAutoCompleteProps<
	T extends IEntryWithIdAndName
> {
	children: React.ReactNode;
	basicEntryLinkStore: BasicEntryLinkStore<T>;
}

const BasicEntryLinkLockingAutoComplete = observer(
	<T extends IEntryWithIdAndName>({
		children,
		basicEntryLinkStore,
	}: BasicEntryLinkLockingAutoCompleteProps<T>): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return basicEntryLinkStore.isEmpty ? (
			<>{children}</>
		) : (
			<div className="input-append">
				<input
					type="text"
					className="input-large"
					readOnly
					value={basicEntryLinkStore.name}
				/>
				<Button variant="danger" onClick={basicEntryLinkStore.clear}>
					{t('ViewRes:Shared.Clear')}
				</Button>
			</div>
		);
	},
);

export default BasicEntryLinkLockingAutoComplete;
