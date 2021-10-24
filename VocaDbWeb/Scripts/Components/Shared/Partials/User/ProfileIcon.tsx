import React from 'react';

interface ProfileIconProps {
	url?: string;
	size?: number;
}

const ProfileIcon = React.memo(
	({ url, size = 80 }: ProfileIconProps): React.ReactElement => {
		return url ? (
			<div
				style={{
					width: `${size}px`,
					height: `${size}px`,
					display: 'inline-block',
				}}
			>
				<img src={url} alt="Icon" /* TODO: localize */ />
			</div>
		) : (
			<></>
		);
	},
);

export default ProfileIcon;
