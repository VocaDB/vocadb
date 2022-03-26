import TagCategoryAutoComplete from '@Components/KnockoutExtensions/TagCategoryAutoComplete';
import React from 'react';
import { useTranslation } from 'react-i18next';

import LockingAutoComplete from './LockingAutoComplete';

interface TagCategoryLockingAutoCompleteProps {
	value?: string;
	onChange: (value?: string) => void;
	clearValue: boolean;
}

const TagCategoryLockingAutoComplete = React.memo(
	({
		value,
		onChange,
		clearValue,
	}: TagCategoryLockingAutoCompleteProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<LockingAutoComplete
				text={value}
				value={value}
				onClear={(): void => onChange(undefined)}
			>
				<TagCategoryAutoComplete
					type="text"
					className="input-large"
					onAcceptSelection={onChange}
					clearValue={clearValue}
					placeholder={t('ViewRes:Shared.Search')}
				/>
			</LockingAutoComplete>
		);
	},
);

export default TagCategoryLockingAutoComplete;
