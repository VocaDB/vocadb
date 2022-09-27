import { ProfileIconKnockout } from '@/Components/Shared/Partials/User/ProfileIconKnockout';
import React from 'react';

interface IconAndNameKnockoutProps {
	icon?: string;
	name?: string;
	size?: number;
}

export const IconAndNameKnockout = React.memo(
	({ icon, name, size = 20 }: IconAndNameKnockoutProps): React.ReactElement => {
		return (
			<>
				<ProfileIconKnockout icon={icon} size={size} /> <span>{name}</span>
			</>
		);
	},
);
