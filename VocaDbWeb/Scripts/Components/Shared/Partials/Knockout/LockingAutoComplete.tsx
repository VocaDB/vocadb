import Button from '@/Bootstrap/Button';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface LockingAutoCompleteProps {
	children: React.ReactNode;
	text?: string;
	value?: any;
	entryType?: EntryType;
	onClear: () => void;
}

// Autocomplete box that allows selection of one item. When an item is selected, "clear" button will be displayed.
export const LockingAutoComplete = React.memo(
	({
		children,
		text,
		value,
		entryType = EntryType.Undefined,
		onClear,
	}: LockingAutoCompleteProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return value ? (
			<div className="input-append">
				{entryType !== EntryType.Undefined && (
					<Button
						as={Link}
						className="btn-nomargin"
						to={EntryUrlMapper.details(entryType, value)}
					>
						<i className="icon icon-info-sign" />
					</Button>
				)}
				<input
					type="text"
					className="input-large"
					readOnly
					value={text ?? ''}
				/>
				<Button variant="danger" onClick={onClear}>
					{t('ViewRes:Shared.Clear')}
				</Button>
			</div>
		) : (
			<>{children}</>
		);
	},
);
