// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/d707ca3a6690b54f0990b581cfe13c44e2a2c891/src/SafeAnchor.tsx

import React from 'react';
import createChainedFunction from './createChainedFunction';
import { BsPrefixProps } from './helpers';

export interface SafeAnchorProps extends React.HTMLAttributes<HTMLElement>, BsPrefixProps {
	href?: string;
	disabled?: boolean;
	role?: string;
	tabIndex?: number;
}

function isTrivialHref(href) {
	return !href || href.trim() === '#';
}

const SafeAnchor = React.forwardRef(
	(
		{
			as: Component = 'a',
			disabled,
			onKeyDown,
			...props
		}: SafeAnchorProps,
		ref,
	) => {
		const handleClick = (event) => {
			const { href, onClick } = props;

			if (disabled || isTrivialHref(href)) {
				event.preventDefault();
			}

			if (disabled) {
				event.stopPropagation();
				return;
			}

			onClick?.(event);
		};

		const handleKeyDown = (event) => {
			if (event.key === ' ') {
				event.preventDefault();
				handleClick(event);
			}
		};

		if (isTrivialHref(props.href)) {
			props.role = props.role || 'button';
			props.href = props.href || '#';
		}

		if (disabled) {
			props.tabIndex = -1;
			props['aria-disabled'] = true;
		}

		return (
			<Component
				ref={ref}
				{...props}
				onClick={handleClick}
				onKeyDown={createChainedFunction(handleKeyDown, onKeyDown)}
			/>
		);
	},
);

export default SafeAnchor;
