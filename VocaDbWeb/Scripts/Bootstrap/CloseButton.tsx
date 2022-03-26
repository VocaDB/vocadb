// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/c11bc67ab3105e7a1839c0dcaacc5f1099885f02/src/CloseButton.tsx
import classNames from 'classnames';
import * as React from 'react';

export type CloseButtonVariant = 'white';

export interface CloseButtonProps
	extends React.ButtonHTMLAttributes<HTMLButtonElement> {
	variant?: CloseButtonVariant;
}

const CloseButton = React.forwardRef<HTMLButtonElement, CloseButtonProps>(
	({ className, variant, ...props }, ref) => (
		<button
			ref={ref}
			type="button"
			className={classNames(
				'close',
				variant && `btn-close-${variant}`,
				className,
			)}
			{...props}
		>
			&times;
		</button>
	),
);

CloseButton.displayName = 'CloseButton';

export default CloseButton;
