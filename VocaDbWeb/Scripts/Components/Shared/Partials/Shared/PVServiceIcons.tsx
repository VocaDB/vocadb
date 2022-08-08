import PVServiceIcon from '@/Components/Shared/Partials/Shared/PVServiceIcon';
import PVService from '@/Models/PVs/PVService';
import React from 'react';

interface PVServiceIconsProps {
	services: string /* TODO: enum */[];
}

const PVServiceIcons = React.memo(
	({ services }: PVServiceIconsProps): React.ReactElement => {
		return (
			<>
				{Object.keys(/* TODO: values */ PVService)
					.filter((service) => services.includes(service))
					.map((service, index) => (
						<React.Fragment key={service}>
							{index > 0 && ' '}
							<PVServiceIcon
								service={PVService[service as keyof typeof PVService]}
							/>
						</React.Fragment>
					))}
			</>
		);
	},
);

export default PVServiceIcons;
