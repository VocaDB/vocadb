import ProfileIconKnockout from '@/Components/Shared/Partials/User/ProfileIconKnockout';
import React from 'react';

interface IconAndNameKnockoutProps {
	icon?: string;
	name?: string;
	size: number;
}

const IconAndNameKnockout = React.memo(
	({ icon, name, size }: IconAndNameKnockoutProps): React.ReactElement => {
		return (
			<>
				<ProfileIconKnockout icon={icon} size={size} /> <span>{name}</span>
			</>
		);
	},
);

export default IconAndNameKnockout;
