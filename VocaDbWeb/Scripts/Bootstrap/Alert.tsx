// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/c11bc67ab3105e7a1839c0dcaacc5f1099885f02/src/Alert.tsx
import useEventCallback from '@restart/hooks/useEventCallback';
import classNames from 'classnames';
import * as React from 'react';
import { useUncontrolled } from 'uncontrollable';

import CloseButton, { CloseButtonVariant } from './CloseButton';
import { useBootstrapPrefix } from './ThemeProvider';
import { Variant } from './types';

export interface AlertProps extends React.HTMLAttributes<HTMLDivElement> {
	bsPrefix?: string;
	variant?: Variant;
	dismissible?: boolean;
	show?: boolean;
	onClose?: (a: any, b: any) => void;
	closeLabel?: string;
	closeVariant?: CloseButtonVariant;
}

const defaultProps = {
	show: true,
	closeLabel: 'Close alert',
};

const Alert = React.forwardRef<HTMLDivElement, AlertProps>(
	(uncontrolledProps: AlertProps, ref) => {
		const {
			bsPrefix,
			show,
			closeLabel,
			closeVariant,
			className,
			children,
			variant,
			onClose,
			dismissible,
			...props
		} = useUncontrolled(uncontrolledProps, {
			show: 'onClose',
		});

		const prefix = useBootstrapPrefix(bsPrefix, 'alert');
		const handleClose = useEventCallback((e) => {
			if (onClose) {
				onClose(false, e);
			}
		});
		const alert = (
			<div
				role="alert"
				{...props}
				ref={ref}
				className={classNames(
					className,
					prefix,
					variant && `${prefix}-${variant}`,
					dismissible && `${prefix}-dismissible`,
				)}
			>
				{dismissible && (
					<CloseButton
						onClick={handleClose}
						aria-label={closeLabel}
						variant={closeVariant}
					/>
				)}
				{children}
			</div>
		);

		return show ? alert : null;
	},
);

Alert.displayName = 'Alert';
Alert.defaultProps = defaultProps;

export default Object.assign(Alert);
