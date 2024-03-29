// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/f62da57493a63e40bd67b74f1414ac025c54d553/src/Accordion.tsx.
import AccordionBody from '@/Bootstrap/AccordionBody';
import AccordionButton from '@/Bootstrap/AccordionButton';
import AccordionCollapse from '@/Bootstrap/AccordionCollapse';
import AccordionContext, {
	AccordionEventKey,
	AccordionSelectCallback,
} from '@/Bootstrap/AccordionContext';
import AccordionHeader from '@/Bootstrap/AccordionHeader';
import AccordionItem from '@/Bootstrap/AccordionItem';
import { useBootstrapPrefix } from '@/Bootstrap/ThemeProvider';
import {
	BsPrefixProps,
	BsPrefixRefForwardingComponent,
} from '@/Bootstrap/helpers';
import classNames from 'classnames';
import PropTypes from 'prop-types';
import * as React from 'react';
import { useMemo } from 'react';
import { useUncontrolled } from 'uncontrollable';

export interface AccordionProps
	extends Omit<React.HTMLAttributes<HTMLElement>, 'onSelect'>,
		BsPrefixProps {
	activeKey?: AccordionEventKey;
	defaultActiveKey?: AccordionEventKey;
	onSelect?: AccordionSelectCallback;
	flush?: boolean;
	alwaysOpen?: boolean;
}

const propTypes = {
	/** Set a custom element for this component */
	as: PropTypes.elementType,

	/** @default 'accordion' */
	bsPrefix: PropTypes.string,

	/** The current active key that corresponds to the currently expanded card */
	activeKey: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),

	/** The default active key that is expanded on start */
	defaultActiveKey: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),

	/** Renders accordion edge-to-edge with its parent container */
	flush: PropTypes.bool,

	/** Allow accordion items to stay open when another item is opened */
	alwaysOpen: PropTypes.bool,
};

const Accordion: BsPrefixRefForwardingComponent<
	'div',
	AccordionProps
> = React.forwardRef<HTMLElement, AccordionProps>((props, ref) => {
	const {
		// Need to define the default "as" during prop destructuring to be compatible with styled-components github.com/react-bootstrap/react-bootstrap/issues/3595
		as: Component = 'div',
		activeKey,
		bsPrefix,
		className,
		onSelect,
		flush,
		alwaysOpen,
		...controlledProps
	} = useUncontrolled(props, {
		activeKey: 'onSelect',
	});

	const prefix = useBootstrapPrefix(bsPrefix, 'accordion');
	const contextValue = useMemo(
		() => ({
			activeEventKey: activeKey,
			onSelect,
			alwaysOpen,
		}),
		[activeKey, onSelect, alwaysOpen],
	);

	return (
		<AccordionContext.Provider value={contextValue}>
			<Component
				ref={ref}
				{...controlledProps}
				className={classNames(className, prefix, flush && `${prefix}-flush`)}
			/>
		</AccordionContext.Provider>
	);
});

Accordion.displayName = 'Accordion';
Accordion.propTypes = propTypes;

export default Object.assign(Accordion, {
	Button: AccordionButton,
	Collapse: AccordionCollapse,
	Item: AccordionItem,
	Header: AccordionHeader,
	Body: AccordionBody,
});
