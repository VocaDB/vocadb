import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';
import React from 'react';

interface MomentJsTimeAgoProps {
	as: React.ElementType;
	className?: string;
	children: string;
}

dayjs.extend(relativeTime);

export const MomentJsTimeAgo = ({
	as: Component,
	className,
	children,
}: MomentJsTimeAgoProps): React.ReactElement => {
	const parsed = dayjs(children);

	return (
		<Component title={parsed.format('l LT ([UTC]Z)')} className={className}>
			{parsed.fromNow()}
		</Component>
	);
};
