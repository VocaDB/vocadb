import $ from 'jquery';
import 'qtip2';
import React from 'react';

interface HelpLabelProps {
	label: string;
	title: string;
	forElem?: string;
}

// Displays label element with attached qTip tooltip
export const HelpLabel = ({
	label,
	title,
	forElem,
}: HelpLabelProps): React.ReactElement => {
	const el = React.useRef<HTMLLabelElement>(undefined!);

	React.useEffect(() => {
		$(el.current).qtip({
			style: { classes: 'tooltip-wider' },
		});

		return (): void => {
			$('.qtip').remove();
		};
	}, []);

	return (
		<label className="helpTip" title={title} htmlFor={forElem ?? ''} ref={el}>
			{label}
		</label>
	);
};
