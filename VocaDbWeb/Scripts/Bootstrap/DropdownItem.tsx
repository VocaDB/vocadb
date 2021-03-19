// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/d707ca3a6690b54f0990b581cfe13c44e2a2c891/src/DropdownItem.tsx

import React from 'react';
import { BsPrefixProps } from './helpers';

export interface DropdownItemProps extends BsPrefixProps, Omit<React.HTMLAttributes<HTMLElement>, 'onSelect'> {

}

const DropdownItem = React.forwardRef(
	(
		{
			as: Component,
			...props
		}: DropdownItemProps,
		ref,
	) => {
		return (
			<Component
				{...props}
			/>
		);
	},
);

export default DropdownItem;
