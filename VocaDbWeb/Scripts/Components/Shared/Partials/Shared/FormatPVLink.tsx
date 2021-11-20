import PVContract from '@DataContracts/PVs/PVContract';
import PVService from '@Models/PVs/PVService';
import React from 'react';
import { useTranslation } from 'react-i18next';

import PVServiceIcon from './PVServiceIcon';

interface FormatPVLinkProps {
	pv: PVContract;
	type?: boolean;
}

const FormatPVLink = React.memo(
	({ pv, type = true }: FormatPVLinkProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<a href={pv.url}>
				<PVServiceIcon
					service={PVService[pv.service as keyof typeof PVService]}
				/>{' '}
				{pv.name || pv.service}
				{type && <> ({t(`Resources:PVTypeNames.${pv.pvType}`)})</>}
			</a>
		);
	},
);

export default FormatPVLink;
