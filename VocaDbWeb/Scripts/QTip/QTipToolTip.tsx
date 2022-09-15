import React from 'react';

type QTipToolTipProps = React.HTMLAttributes<HTMLDivElement>;

export const QTipToolTip = React.forwardRef<HTMLDivElement, QTipToolTipProps>(
	({ children, ...props }: QTipToolTipProps, ref): React.ReactElement => {
		return (
			<div
				ref={ref}
				className="qtip qtip-default tooltip-wide qtip-pos-tl qtip-focus"
				style={{
					...props.style,
					display: 'block',
					zIndex: 15001,
				}}
			>
				<div className="qtip-content">{children}</div>
			</div>
		);
	},
);
