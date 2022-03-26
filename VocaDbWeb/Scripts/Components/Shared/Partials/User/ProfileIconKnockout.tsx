import React from 'react';

interface ProfileIconKnockoutProps {
	icon?: string;
	size: number;
}

const ProfileIconKnockout = React.memo(
	({ icon, size }: ProfileIconKnockoutProps): React.ReactElement => {
		return (
			<div
				style={{
					width: `${size}px`,
					height: `${size}px`,
					display: 'inline-block',
				}}
			>
				<img src={icon ?? '/Content/unknown.png'} alt="User avatar" />
			</div>
		);
	},
);

export default ProfileIconKnockout;
