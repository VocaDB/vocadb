import Button from '@/Bootstrap/Button';
import { useVdb } from '@/VdbContext';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SongTypesDropdownKnockoutProps {
	activeKey: string;
	onSelect: (eventKey: string) => void;
}

export const SongTypesDropdownKnockout = React.memo(
	({
		activeKey,
		onSelect,
	}: SongTypesDropdownKnockoutProps): React.ReactElement => {
		const vdb = useVdb();

		const { t } = useTranslation(['VocaDb.Model.Resources.Songs']);

		return (
			<div style={{ display: 'inline-block' }}>
				{vdb.values.songTypes.map((songType, index) => (
					<React.Fragment key={songType}>
						{index > 0 && ' '}
						<Button
							onClick={(): void => onSelect(songType)}
							disabled={songType === activeKey}
						>
							{t(`VocaDb.Model.Resources.Songs:SongTypeNames.${songType}`)}
						</Button>
					</React.Fragment>
				))}
			</div>
		);
	},
);
