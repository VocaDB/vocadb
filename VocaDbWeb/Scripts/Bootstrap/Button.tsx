// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/d707ca3a6690b54f0990b581cfe13c44e2a2c891/src/Button.tsx

import classNames from 'classnames';
import React from 'react';
import { BsPrefixPropsWithChildren } from './helpers';
import { useBootstrapPrefix } from './ThemeProvider';
import SafeAnchor from './SafeAnchor';
import { ButtonVariant } from './types';

export interface ButtonProps extends React.HTMLAttributes<HTMLElement>, BsPrefixPropsWithChildren {
	variant?: ButtonVariant;
	href?: string;
}

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
	({ bsPrefix, variant, className, as, ...props }, ref) => {
		const prefix = useBootstrapPrefix(bsPrefix, 'btn');

		const classes = classNames(
			className,
			variant && `${prefix}-${variant}`,
			prefix,
		);

		if (props.href) {
			return (
				<SafeAnchor
					{...props}
					as={as}
					ref={ref}
					className={classNames(classes)}
				/>
			);
		}

		const Component = as || 'button';
		return <Component {...props} ref={ref} className={classes} />;
	},
);

export default Button;
