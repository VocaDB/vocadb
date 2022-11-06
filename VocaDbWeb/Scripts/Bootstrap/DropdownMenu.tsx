// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/8a7e095e8032fdeac4fd1fdb41e6dfb452ae4494/src/DropdownMenu.tsx
import { useBootstrapPrefix } from '@/Bootstrap/ThemeProvider';
import {
	BsPrefixProps,
	BsPrefixRefForwardingComponent,
} from '@/Bootstrap/helpers';
import { AlignDirection, AlignType } from '@/Bootstrap/types';
import { SelectCallback } from '@restart/ui/esm/types';
import classNames from 'classnames';
import * as React from 'react';
import { useDropdownMenu } from 'react-overlays/DropdownMenu';
import warning from 'warning';

export interface DropdownMenuProps
	extends BsPrefixProps,
		Omit<React.HTMLAttributes<HTMLElement>, 'onSelect'> {
	show?: boolean;
	align?: AlignType;
	onSelect?: SelectCallback;
	rootCloseEvent?: 'click' | 'mousedown';
}

const DropdownMenu: BsPrefixRefForwardingComponent<
	'ul',
	DropdownMenuProps
> = React.forwardRef<HTMLElement, DropdownMenuProps>(
	(
		{
			bsPrefix,
			className,
			align,
			rootCloseEvent,
			show: showProps,
			// Need to define the default "as" during prop destructuring to be compatible with styled-components github.com/react-bootstrap/react-bootstrap/issues/3595
			as: Component = 'ul',
			...props
		},
		ref,
	) => {
		let alignRight = false;
		const prefix = useBootstrapPrefix(bsPrefix, 'dropdown-menu');

		const alignClasses: string[] = [];
		if (align) {
			if (typeof align === 'object') {
				const keys = Object.keys(align);

				warning(
					keys.length === 1,
					'There should only be 1 breakpoint when passing an object to `align`',
				);

				if (keys.length) {
					const brkPoint = keys[0];
					const direction: AlignDirection = (align as any)[brkPoint];

					// .dropdown-menu-end is required for responsively aligning
					// left in addition to align left classes.
					// Reuse alignRight to toggle the class below.
					alignRight = direction === 'start';
					alignClasses.push(`${prefix}-${brkPoint}-${direction}`);
				}
			} else if (align === 'end') {
				alignRight = true;
			}
		}

		const [menuProps, { show, alignEnd }] = useDropdownMenu({
			rootCloseEvent,
			show: showProps,
			alignEnd: alignRight,
		});

		return (
			<Component
				{...props}
				{...menuProps}
				// Bootstrap css requires this data attrib to style responsive menus.
				className={classNames(
					className,
					prefix,
					show && 'show',
					alignEnd && `${prefix}-end`,
				)}
			/>
		);
	},
);

export default DropdownMenu;
