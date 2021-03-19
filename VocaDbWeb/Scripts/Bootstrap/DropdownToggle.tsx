// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/d707ca3a6690b54f0990b581cfe13c44e2a2c891/src/DropdownToggle.tsx

import classNames from 'classnames';
import React from 'react';
import { useDropdownToggle } from 'react-overlays/esm/DropdownToggle';
import Button, { ButtonProps } from './Button';
import { useBootstrapPrefix } from './ThemeProvider';

export interface DropdownToggleProps extends ButtonProps {

}

const DropdownToggle = React.forwardRef(
	(
		{
			bsPrefix,
			className,
			as: Component = Button,
			...props
		}: DropdownToggleProps,
		ref,
	) => {
		const prefix = useBootstrapPrefix(bsPrefix, 'dropdown-toggle');

		const [toggleProps, { toggle }] = useDropdownToggle();

		return (
			<Component
				onClick={toggle}
				className={classNames(className, prefix)}
				{...toggleProps}
				{...props}
			/>
		);
	}
);

export default DropdownToggle;
