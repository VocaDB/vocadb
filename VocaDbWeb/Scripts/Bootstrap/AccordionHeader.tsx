// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/f62da57493a63e40bd67b74f1414ac025c54d553/src/AccordionHeader.tsx.
import AccordionButton from '@/Bootstrap/AccordionButton';
import { useBootstrapPrefix } from '@/Bootstrap/ThemeProvider';
import {
	BsPrefixProps,
	BsPrefixRefForwardingComponent,
} from '@/Bootstrap/helpers';
import classNames from 'classnames';
import PropTypes from 'prop-types';
import * as React from 'react';

export interface AccordionHeaderProps
	extends BsPrefixProps,
		React.HTMLAttributes<HTMLElement> {}

const propTypes = {
	/** Set a custom element for this component */
	as: PropTypes.elementType,

	/** @default 'accordion-header' */
	bsPrefix: PropTypes.string,

	/** Click handler for the `AccordionButton` element */
	onClick: PropTypes.func,
};

const AccordionHeader: BsPrefixRefForwardingComponent<
	'div',
	AccordionHeaderProps
> = React.forwardRef<HTMLElement, AccordionHeaderProps>(
	(
		{
			// Need to define the default "as" during prop destructuring to be compatible with styled-components github.com/react-bootstrap/react-bootstrap/issues/3595
			as: Component = 'div',
			bsPrefix,
			className,
			children,
			onClick,
			...props
		},
		ref,
	) => {
		bsPrefix = useBootstrapPrefix(bsPrefix, 'accordion-heading');

		return (
			<Component
				ref={ref}
				{...props}
				className={classNames(className, bsPrefix)}
			>
				<AccordionButton onClick={onClick}>{children}</AccordionButton>
			</Component>
		);
	},
);

AccordionHeader.propTypes = propTypes;
AccordionHeader.displayName = 'AccordionHeader';

export default AccordionHeader;
