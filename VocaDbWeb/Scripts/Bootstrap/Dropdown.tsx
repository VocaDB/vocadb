// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/d707ca3a6690b54f0990b581cfe13c44e2a2c891/src/Dropdown.tsx

import React from 'react';
import BaseDropdown from 'react-overlays/Dropdown';
import { useUncontrolled } from 'uncontrollable';
import useEventCallback from '@restart/hooks/useEventCallback';
import SelectableContext from './SelectableContext'
import { BsPrefixProps } from './helpers';
import { useBootstrapPrefix } from './ThemeProvider';
import classNames from 'classnames';
import DropdownToggle from './DropdownToggle';
import DropdownMenu from './DropdownMenu';
import DropdownItem from './DropdownItem';

export interface DropdownProps extends BsPrefixProps, Omit<React.HTMLAttributes<HTMLElement>, 'onSelect'> {
	drop?: 'up' | 'start' | 'end' | 'down';
	show?: boolean;
	onToggle?: (
		isOpen: boolean,
		event: React.SyntheticEvent,
		metadata: { source: 'select' | 'click' | 'rootClose' | 'keydown' },
	) => void;
}

const Dropdown = React.forwardRef<HTMLElement, DropdownProps>((pProps, ref) => {
	const {
		bsPrefix,
		show,
		className,
		onToggle,
		as: Component = 'div',
		...props
	} = useUncontrolled(pProps, { show: 'onToggle' });

	const prefix = useBootstrapPrefix(bsPrefix, 'dropdown');

	const handleToggle = useEventCallback(
		(nextShow, event, source = event.type) => {
			if (event.currentTarget === document) source = 'rootClose';
			onToggle?.(nextShow, event, { source });
		},
	);

	const handleSelect = useEventCallback((key, event) => {

	});

	return (
		<SelectableContext.Provider value={handleSelect}>
			<BaseDropdown
				show={show}
				onToggle={handleToggle}
				itemSelector={`.${prefix}-item:not(.disabled):not(:disabled)`}
			>
				{({ props: dropdownProps }) => (
					<Component
						{...props}
						ref={ref}
						className={classNames(
							className,
							show && 'open',
							"btn-group",
						)}
					/>
				)}
			</BaseDropdown>
		</SelectableContext.Provider>
	);
});

export default Object.assign(Dropdown, {
	Toggle: DropdownToggle,
	Menu: DropdownMenu,
	Item: DropdownItem,
});
