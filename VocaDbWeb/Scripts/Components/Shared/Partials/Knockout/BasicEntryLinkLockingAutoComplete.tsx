import Button from '@Bootstrap/Button';
import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface BasicEntryLinkLockingAutoCompleteProps<
	T extends IEntryWithIdAndName
> {
	as: React.ElementType;
	basicEntryLinkStore: BasicEntryLinkStore<T>;
}

const BasicEntryLinkLockingAutoComplete = observer(
	<T extends IEntryWithIdAndName>({
		as: Component,
		basicEntryLinkStore,
	}: BasicEntryLinkLockingAutoCompleteProps<T>): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<>
				{basicEntryLinkStore.isEmpty ? (
					<Component
						type="text"
						className="input-large"
						onAcceptSelection={(entry: T): void => {
							runInAction(() => (basicEntryLinkStore.entry = entry));
						}}
						placeholder={t('ViewRes:Shared.Search')}
					/>
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
				)}
			</>
		);
	},
);

export default BasicEntryLinkLockingAutoComplete;
