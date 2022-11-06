import { PVServiceIcon } from '@/Components/Shared/Partials/Shared/PVServiceIcon';
import { PVService } from '@/Models/PVs/PVService';
import React from 'react';

interface PVServiceIconsProps {
	services: PVService[];
}

export const PVServiceIcons = React.memo(
	({ services }: PVServiceIconsProps): React.ReactElement => {
		return (
			<>
				{Object.values(PVService)
					.filter((service) => services.includes(service))
					.map((service, index) => (
						<React.Fragment key={service}>
							{index > 0 && ' '}
							<PVServiceIcon service={service} />
						</React.Fragment>
					))}
			</>
		);
	},
);
