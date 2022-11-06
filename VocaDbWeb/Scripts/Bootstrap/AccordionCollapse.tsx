// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/f62da57493a63e40bd67b74f1414ac025c54d553/src/AccordionCollapse.tsx.
import AccordionContext, {
	isAccordionItemSelected,
} from '@/Bootstrap/AccordionContext';
import Collapse, { CollapseProps } from '@/Bootstrap/Collapse';
import { useBootstrapPrefix } from '@/Bootstrap/ThemeProvider';
import {
	BsPrefixProps,
	BsPrefixRefForwardingComponent,
} from '@/Bootstrap/helpers';
import classNames from 'classnames';
import PropTypes from 'prop-types';
import * as React from 'react';
import { useContext } from 'react';
import { Transition } from 'react-transition-group';

export interface AccordionCollapseProps extends BsPrefixProps, CollapseProps {
	eventKey: string;
}

const propTypes = {
	/** Set a custom element for this component */
	as: PropTypes.elementType,

	/**
	 * A key that corresponds to the toggler that triggers this collapse's expand or collapse.
	 */
	eventKey: PropTypes.string.isRequired,

	/** Children prop should only contain a single child, and is enforced as such */
	children: PropTypes.element.isRequired,
};

const AccordionCollapse: BsPrefixRefForwardingComponent<
	'div',
	AccordionCollapseProps
> = React.forwardRef<Transition<any>, AccordionCollapseProps>(
	(
		{
			as: Component = 'div',
			bsPrefix,
			className,
			children,
			eventKey,
			...props
		},
		ref,
	) => {
		const { activeEventKey } = useContext(AccordionContext);
		bsPrefix = useBootstrapPrefix(bsPrefix, 'accordion-collapse');

		return (
			<Collapse
				ref={ref}
				in={isAccordionItemSelected(activeEventKey, eventKey)}
				{...props}
				className={classNames(className, bsPrefix)}
			>
				<Component>{React.Children.only(children)}</Component>
			</Collapse>
		);
	},
) as any;

AccordionCollapse.propTypes = propTypes;
AccordionCollapse.displayName = 'AccordionCollapse';

export default AccordionCollapse;
