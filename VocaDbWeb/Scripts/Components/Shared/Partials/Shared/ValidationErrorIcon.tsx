import OverlayTrigger from '@/Bootstrap/OverlayTrigger';
import { QTipToolTip } from '@/QTip/QTipToolTip';
import React from 'react';

interface ValidationErrorIconProps {
	title: string;
}

// Displays label element with attached qTip tooltip
export const ValidationErrorIcon = ({
	title,
}: ValidationErrorIconProps): React.ReactElement => {
	return (
		<OverlayTrigger
			placement="bottom-start"
			delay={{ show: 250, hide: 0 }}
			flip
			offset={[0, 8]}
			overlay={<QTipToolTip style={{ opacity: 1 }}>{title}</QTipToolTip>}
		>
			<span>
				<span className="icon errorIcon" />
			</span>
		</OverlayTrigger>
	);
};
