import $ from 'jquery';
import 'qtip2';
import React from 'react';

interface ValidationErrorIconProps {
	title: string;
}

// Displays label element with attached qTip tooltip
export const ValidationErrorIcon = ({
	title,
}: ValidationErrorIconProps): React.ReactElement => {
	const el = React.useRef<HTMLLabelElement>(undefined!);

	React.useEffect(() => {
		$(el.current).qtip({
			style: { classes: 'tooltip-wider' },
		});

		return (): void => {
			$('.qtip').remove();
		};
	}, []);

	return <span className="icon errorIcon" title={title} ref={el} />;
};
