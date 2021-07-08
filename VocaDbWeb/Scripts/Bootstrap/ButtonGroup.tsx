// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/3d4c57374646949e6fedfef00236c99f4d1b4e71/src/ButtonGroup.tsx
import classNames from 'classnames';
import * as React from 'react';

import { useBootstrapPrefix } from './ThemeProvider';
import { BsPrefixProps, BsPrefixRefForwardingComponent } from './helpers';

export interface ButtonGroupProps
	extends BsPrefixProps,
		React.HTMLAttributes<HTMLElement> {
	size?: 'sm' | 'lg';
	vertical?: boolean;
}

const ButtonGroup: BsPrefixRefForwardingComponent<
	'div',
	ButtonGroupProps
> = React.forwardRef(
	(
		{
			bsPrefix,
			size,
			vertical,
			className,
			// Need to define the default "as" during prop destructuring to be compatible with styled-components github.com/react-bootstrap/react-bootstrap/issues/3595
			as: Component = 'div',
			...rest
		}: ButtonGroupProps,
		ref,
	) => {
		const prefix = useBootstrapPrefix(bsPrefix, 'btn-group');
		let baseClass = prefix;

		if (vertical) baseClass = `${prefix}-vertical`;

		return (
			<Component
				{...rest}
				ref={ref}
				className={classNames(
					className,
					baseClass,
					size && `${prefix}-${size}`,
				)}
			/>
		);
	},
);

ButtonGroup.displayName = 'ButtonGroup';

export default ButtonGroup;
