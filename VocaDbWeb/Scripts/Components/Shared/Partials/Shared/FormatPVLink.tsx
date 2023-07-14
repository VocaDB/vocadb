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
			<>
				{!pv.disabled ? (
					<a href={pv.url}>
						<PVServiceIcon service={pv.service} /> {pv.name || pv.service}
						{type && <> ({t(`Resources:PVTypeNames.${pv.pvType}`)})</>}
					</a>
				) : (
					<a
						title="See this page archived on the Wayback Machine" /* LOC */
						href={`https://web.archive.org/web/*/$${pv.url}`}
						style={{ textDecoration: 'line-through' }}
					>
						<img src="/Content/ExtIcons/archive.png" alt="Internet Archive" />{' '}
						{pv.name || pv.service}
						{type && <> ({t(`Resources:PVTypeNames.${pv.pvType}`)})</>}
					</a>
				)}
			</>
		);
	},
);
