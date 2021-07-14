import moment from 'moment';
import React from 'react';

interface MomentJsTimeAgoProps {
	as: React.ElementType;
	className: string;
	children?: Date;
}

const MomentJsTimeAgo = ({
	as: Component,
	className,
	children,
}: MomentJsTimeAgoProps): React.ReactElement => {
	const parsed = moment(children);

	return (
		<Component title={parsed.format('l LT ([UTC]Z)')} className={className}>
			{parsed.fromNow()}
		</Component>
	);
};

export default MomentJsTimeAgo;
