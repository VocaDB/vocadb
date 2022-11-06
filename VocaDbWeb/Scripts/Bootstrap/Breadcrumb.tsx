// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/3d4c57374646949e6fedfef00236c99f4d1b4e71/src/Breadcrumb.tsx
import BreadcrumbItem from '@/Bootstrap/BreadcrumbItem';
import { useBootstrapPrefix } from '@/Bootstrap/ThemeProvider';
import {
	BsPrefixProps,
	BsPrefixRefForwardingComponent,
} from '@/Bootstrap/helpers';
import classNames from 'classnames';
import * as React from 'react';

export interface BreadcrumbProps
	extends BsPrefixProps,
		React.HTMLAttributes<HTMLElement> {
	label?: string;
	listProps?: React.OlHTMLAttributes<HTMLOListElement>;
}

const Breadcrumb: BsPrefixRefForwardingComponent<
	'nav',
	BreadcrumbProps
> = React.forwardRef<HTMLElement, BreadcrumbProps>(
	(
		{
			bsPrefix,
			className,
			listProps,
			children,
			label,
			// Need to define the default "as" during prop destructuring to be compatible with styled-components github.com/react-bootstrap/react-bootstrap/issues/3595
			as: Component = 'nav',
			...props
		},
		ref,
	) => {
		const prefix = useBootstrapPrefix(bsPrefix, 'breadcrumb');

		return (
			<Component aria-label={label} className={className} ref={ref} {...props}>
				<ol {...listProps} className={classNames(prefix, listProps?.className)}>
					{children}
				</ol>
			</Component>
		);
	},
);

Breadcrumb.displayName = 'Breadcrumb';

export default Object.assign(Breadcrumb, {
	Item: BreadcrumbItem,
});
