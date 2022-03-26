import React from 'react';

import ProfileIconKnockout from './ProfileIconKnockout';

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
