// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/8e3f3c2b5f232e17829df3474b7b6e398f22ff3d/src/Tabs.tsx
import { forEach, map } from '@Bootstrap/ElementChildren';
import NavbarContext from '@Bootstrap/NavbarContext';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import BaseNav from '@restart/ui/Nav';
import { useNavItem } from '@restart/ui/NavItem';
import { makeEventKey } from '@restart/ui/SelectableContext';
import BaseTabs, { TabsProps as BaseTabsProps } from '@restart/ui/Tabs';
import classNames from 'classnames';
import * as React from 'react';
import { useUncontrolled } from 'uncontrollable';

/*import JQueryUINav from './JQueryUINav';
import JQueryUINavItem from './JQueryUINavItem';
import JQueryUINavLink from './JQueryUINavLink';*/
import JQueryUITabContent from './JQueryUITabContent';
import JQueryUITabPane from './JQueryUITabPane';

const JQueryUINav = (uncontrolledProps: any): React.ReactElement => {
	const {
		as = 'div',
		bsPrefix: initialBsPrefix,
		variant,
		fill,
		justify,
		navbar,
		navbarScroll,
		className,
		activeKey,
		...props
	} = useUncontrolled(uncontrolledProps, { activeKey: 'onSelect' });

	React.useContext(NavbarContext);

	return (
		<BaseNav
			as={as}
			activeKey={activeKey}
			className="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all"
			{...props}
		/>
	);
};

const JQueryUINavItem = ({
	active,
	as: Component,
	children,
	eventKey,
	...props
}: any): React.ReactElement => {
	const [navItemProps, meta] = useNavItem({
		key: makeEventKey(eventKey, props.href),
		active,
		...props,
	});

	return (
		<Component
			{...props}
			{...navItemProps}
			className={classNames(
				'ui-state-default',
				'ui-corner-top',
				meta.isActive && ['ui-tabs-active', 'ui-state-active'],
			)}
		>
			{children}
		</Component>
	);
};

const JQueryUINavLink = ({
	as: Component,
	children,
	...props
}: any): React.ReactElement => {
	return (
		<Component {...props} className="ui-tabs-anchor">
			{children}
		</Component>
	);
};

export interface JQueryUITabsProps
	extends Omit<BaseTabsProps, 'transition'>,
		Omit<React.HTMLAttributes<HTMLElement>, 'onSelect'> {}

function getDefaultActiveKey(children: any): any {
	let defaultActiveKey: any;
	forEach(children, (child) => {
		if (defaultActiveKey == null) {
			defaultActiveKey = child.props.eventKey;
		}
	});

	return defaultActiveKey;
}

function renderTab(child: any): React.ReactElement | null {
	const { title, eventKey, disabled, tabClassName, id } = child.props;
	if (title == null) {
		return null;
	}

	return (
		<JQueryUINavItem as="li" eventKey={eventKey} role="tab">
			<JQueryUINavLink
				as={SafeAnchor}
				href="#"
				disabled={disabled}
				id={id}
				className={tabClassName}
				role="presentation"
			>
				{title}
			</JQueryUINavLink>
		</JQueryUINavItem>
	);
}

const JQueryUITabs = (props: JQueryUITabsProps): React.ReactElement => {
	const {
		id,
		onSelect,
		mountOnEnter,
		unmountOnExit,
		children,
		activeKey = getDefaultActiveKey(children),
		...controlledProps
	} = useUncontrolled(props, {
		activeKey: 'onSelect',
	});

	return (
		<div className="ui-tabs ui-widget ui-widget-content ui-corner-all">
			<BaseTabs
				id={id}
				activeKey={activeKey}
				onSelect={onSelect}
				mountOnEnter={mountOnEnter}
				unmountOnExit={unmountOnExit}
			>
				<JQueryUINav {...controlledProps} role="tablist" as="ul">
					{map(children, renderTab)}
				</JQueryUINav>

				<JQueryUITabContent>
					{map(children, (child) => {
						const childProps = { ...child.props };

						delete childProps.title;
						delete childProps.disabled;
						delete childProps.tabClassName;

						return <JQueryUITabPane {...childProps} />;
					})}
				</JQueryUITabContent>
			</BaseTabs>
		</div>
	);
};

export default JQueryUITabs;
