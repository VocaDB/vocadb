import classNames from 'classnames';
import React from 'react';
import Draggable from 'react-draggable';
import { Modal } from 'react-overlays';

import JQueryUIButton from './JQueryUIButton';

type JQueryUIDialogProps = {
	autoOpen?: boolean;
	buttons?: {
		text: string;
		click: () => void;
		disabled?: boolean;
		icons?: any;
	}[];
	children?: React.ReactNode;
	close?: () => void;
	modal?: boolean;
	width: number;
} & React.InputHTMLAttributes<HTMLDivElement>;

const JQueryUIDialog = ({
	autoOpen,
	buttons,
	children,
	close,
	modal,
	title,
	width,
	height,
	...props
}: JQueryUIDialogProps): React.ReactElement => {
	React.useEffect(() => {
		document.body.style.overflow = '';
		document.body.style.paddingRight = '';
	});

	return (
		<Modal
			show={autoOpen}
			onHide={close}
			className="ui-front"
			style={{
				display: 'flex',
				justifyContent: 'center',
				alignItems: 'center',
				position: 'fixed',
				inset: 0,
				paddingBottom: '10vh',
			}}
			restoreFocusOptions={{ preventScroll: true }}
		>
			<>
				<div className={classNames('ui-widget-overlay', 'ui-front')} />
				<Draggable
					bounds="parent"
					handle=".ui-dialog-titlebar"
					cancel=".ui-dialog-titlebar-close"
				>
					<div
						{...props}
						className={classNames(
							'ui-dialog',
							'ui-widget',
							'ui-widget-content',
							'ui-corner-all',
							'ui-front',
							'ui-dialog-buttons',
							'ui-draggable',
						)}
						role="dialog"
						style={{
							display: 'flex',
							flexDirection: 'column',
							maxHeight: '75vh',
							width: width,
							height: height,
						}}
					>
						<div
							className={classNames(
								'ui-dialog-titlebar',
								'ui-widget-header',
								'ui-corner-all',
							)}
						>
							<span className="ui-dialog-title">{title}</span>
							<button
								type="button"
								className={classNames(
									'ui-button',
									'ui-widget',
									'ui-state-default',
									'ui-corner-all',
									'ui-button-icon-only',
									'ui-dialog-titlebar-close',
								)}
								title="close" /* TODO: localize */
								onClick={close}
							>
								<span
									className={classNames(
										'ui-button-icon-primary',
										'ui-icon',
										'ui-icon-closethick',
									)}
								/>
								<span className="ui-button-text">
									close{/* TODO: localize */}
								</span>
							</button>
						</div>
						<div
							style={{
								width: 'auto',
								minHeight: '32px',
								maxHeight: 'none',
								height: 'auto',
							}}
							className={classNames('ui-dialog-content', 'ui-widget-content')}
						>
							{children}
						</div>
						{buttons && (
							<div
								className={classNames(
									'ui-dialog-buttonpane',
									'ui-widget-content',
								)}
							>
								<div className="ui-dialog-buttonset">
									{buttons.map((button, index) => (
										<JQueryUIButton
											as="button"
											onClick={button.click}
											disabled={button.disabled}
											key={index}
											icons={button.icons}
										>
											{button.text}
										</JQueryUIButton>
									))}
								</div>
							</div>
						)}
					</div>
				</Draggable>
			</>
		</Modal>
	);
};

export default JQueryUIDialog;
