import Button from '@/Bootstrap/Button';
import { useVdb } from '@/VdbContext';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface DiscTypesDropdownKnockoutProps {
	activeKey: string;
	onSelect: (eventKey: string) => void;
}

export const DiscTypesDropdownKnockout = React.memo(
	({
		activeKey,
		onSelect,
	}: DiscTypesDropdownKnockoutProps): React.ReactElement => {
		const vdb = useVdb();

		const { t } = useTranslation(['VocaDb.Model.Resources.Albums']);

		return (
			<div style={{ display: 'inline-block' }}>
				{vdb.values.albumTypes.map((albumType, index) => (
					<React.Fragment key={albumType}>
						{index > 0 && ' '}
						<Button
							onClick={(): void => onSelect(albumType)}
							disabled={albumType === activeKey}
						>
							{t(`VocaDb.Model.Resources.Albums:DiscTypeNames.${albumType}`)}
						</Button>
					</React.Fragment>
				))}
			</div>
		);
	},
);
