// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/3d4c57374646949e6fedfef00236c99f4d1b4e71/src/AbstractNav.tsx
import NavContext from '@/Bootstrap/NavContext';
import SelectableContext, { makeEventKey } from '@/Bootstrap/SelectableContext';
import TabContext from '@/Bootstrap/TabContext';
import {
	BsPrefixRefForwardingComponent,
	SelectCallback,
} from '@/Bootstrap/helpers';
import { EventKey } from '@/Bootstrap/types';
import * as React from 'react';
import { useContext } from 'react';

// eslint-disable-next-line @typescript-eslint/no-empty-function
const noop = (): void => {};

interface AbstractNavProps
	extends Omit<React.HTMLAttributes<HTMLElement>, 'onSelect'> {
	activeKey?: EventKey;
	as?: React.ElementType;
	onSelect?: SelectCallback;
	parentOnSelect?: SelectCallback;
}

const AbstractNav: BsPrefixRefForwardingComponent<'ul', AbstractNavProps> =
	React.forwardRef<HTMLElement, AbstractNavProps>(
		(
			{
				// Need to define the default "as" during prop destructuring to be compatible with styled-components github.com/react-bootstrap/react-bootstrap/issues/3595
				as: Component = 'ul',
				onSelect,
				activeKey,
				role,
				onKeyDown,
				...props
			},
			ref,
		) => {
			const parentOnSelect = useContext(SelectableContext);
			const tabContext = useContext(TabContext);

			let getControlledId, getControllerId;

			if (tabContext) {
				role = role || 'tablist';
				activeKey = tabContext.activeKey;
				getControlledId = tabContext.getControlledId;
				getControllerId = tabContext.getControllerId;
			}

			const handleSelect = (key: any, event: any): void => {
				//if (key == null) return;
				onSelect?.(key, event);
				parentOnSelect?.(key, event);
			};

			return (
				<SelectableContext.Provider value={handleSelect}>
					<NavContext.Provider
						value={{
							role, // used by NavLink to determine it's role
							activeKey: makeEventKey(activeKey),
							getControlledId: getControlledId || noop,
							getControllerId: getControllerId || noop,
						}}
					>
						<Component {...props} ref={ref} role={role} />
					</NavContext.Provider>
				</SelectableContext.Provider>
			);
		},
	);

export default AbstractNav;
