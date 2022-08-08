// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/f62da57493a63e40bd67b74f1414ac025c54d553/src/AccordionBody.tsx.
import AccordionCollapse from '@/Bootstrap/AccordionCollapse';
import AccordionItemContext from '@/Bootstrap/AccordionItemContext';
import { useBootstrapPrefix } from '@/Bootstrap/ThemeProvider';
import {
	BsPrefixProps,
	BsPrefixRefForwardingComponent,
} from '@/Bootstrap/helpers';
import classNames from 'classnames';
import PropTypes from 'prop-types';
import * as React from 'react';
import { useContext } from 'react';

export interface AccordionBodyProps
	extends BsPrefixProps,
		React.HTMLAttributes<HTMLElement> {}

const propTypes = {
	/** Set a custom element for this component */
	as: PropTypes.elementType,

	/** @default 'accordion-body' */
	bsPrefix: PropTypes.string,
};

const AccordionBody: BsPrefixRefForwardingComponent<
	'div',
	AccordionBodyProps
> = React.forwardRef<HTMLElement, AccordionBodyProps>(
	(
		{
			// Need to define the default "as" during prop destructuring to be compatible with styled-components github.com/react-bootstrap/react-bootstrap/issues/3595
			as: Component = 'div',
			bsPrefix,
			className,
			...props
		},
		ref,
	) => {
		bsPrefix = useBootstrapPrefix(bsPrefix, 'accordion-body');
		const { eventKey } = useContext(AccordionItemContext);

		return (
			<AccordionCollapse eventKey={eventKey}>
				<Component
					ref={ref}
					{...props}
					className={classNames(className, bsPrefix)}
				/>
			</AccordionCollapse>
		);
	},
);

AccordionBody.propTypes = propTypes;
AccordionBody.displayName = 'AccordionBody';

export default AccordionBody;
