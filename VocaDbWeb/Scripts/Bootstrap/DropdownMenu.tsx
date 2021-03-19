// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/d707ca3a6690b54f0990b581cfe13c44e2a2c891/src/DropdownMenu.tsx

import classNames from 'classnames';
import React from 'react';
import { useDropdownMenu } from 'react-overlays/esm/DropdownMenu';
import { BsPrefixProps } from './helpers';
import { useBootstrapPrefix } from './ThemeProvider';

export interface DropdownMenuProps extends BsPrefixProps, Omit<React.HTMLAttributes<HTMLElement>, 'onSelect'> {
	show?: boolean;
}

const DropdownMenu = React.forwardRef<HTMLElement, DropdownMenuProps>(
	(
		{
			bsPrefix,
			className,
			show: showProps,
			as: Component = 'div',
			...props
		},
		ref,
	) => {
		const prefix = useBootstrapPrefix(bsPrefix, 'dropdown-menu');

		const {
			show,
			props: menuProps,
		} = useDropdownMenu({
			show: showProps,
		});

		return (
			<Component
				{...props}
				{...menuProps}
				className={classNames(
					className,
					prefix,
					show && 'show',
				)}
			/>
		);
	},
);

export default DropdownMenu;
