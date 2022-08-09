// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/c11bc67ab3105e7a1839c0dcaacc5f1099885f02/src/Pagination.tsx
import PageItem, {
	First,
	Prev,
	Ellipsis,
	Next,
	Last,
} from '@/Bootstrap/PageItem';
import { useBootstrapPrefix } from '@/Bootstrap/ThemeProvider';
import { BsPrefixProps } from '@/Bootstrap/helpers';
import classNames from 'classnames';
import * as React from 'react';

export interface PaginationProps
	extends BsPrefixProps,
		React.HTMLAttributes<HTMLUListElement> {
	size?: 'sm' | 'lg';
}

/**
 * @property {PageItem} Item
 * @property {PageItem} First
 * @property {PageItem} Prev
 * @property {PageItem} Ellipsis
 * @property {PageItem} Next
 * @property {PageItem} Last
 */
const Pagination = React.forwardRef<HTMLUListElement, PaginationProps>(
	({ bsPrefix, className, size, ...props }, ref) => {
		const decoratedBsPrefix = useBootstrapPrefix(bsPrefix, 'pagination');
		return (
			<div className={classNames(decoratedBsPrefix)}>
				<ul
					ref={ref}
					{...props}
					className={classNames(
						className,
						size && `${decoratedBsPrefix}-${size}`,
					)}
				/>
			</div>
		);
	},
);

Pagination.displayName = 'Pagination';

export default Object.assign(Pagination, {
	First,
	Prev,
	Ellipsis,
	Item: PageItem,
	Next,
	Last,
});
