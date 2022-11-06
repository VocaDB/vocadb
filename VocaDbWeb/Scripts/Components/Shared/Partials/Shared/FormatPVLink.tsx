import { PVServiceIcon } from '@/Components/Shared/Partials/Shared/PVServiceIcon';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface FormatPVLinkProps {
	pv: PVContract;
	type?: boolean;
}

export const FormatPVLink = React.memo(
	({ pv, type = true }: FormatPVLinkProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<a href={pv.url}>
				<PVServiceIcon service={pv.service} /> {pv.name || pv.service}
				{type && <> ({t(`Resources:PVTypeNames.${pv.pvType}`)})</>}
			</a>
		);
	},
);
