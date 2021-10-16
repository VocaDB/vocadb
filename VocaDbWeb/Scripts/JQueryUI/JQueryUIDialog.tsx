import React from 'react';
import { Modal } from 'react-overlays';

import JQueryUIButton from './JQueryUIButton';

type JQueryUIDialogProps = {
	autoOpen?: boolean;
	buttons: { text: string; click: () => void }[];
	children?: React.ReactNode;
	close?: () => void;
	modal?: boolean;
	width?: number;
} & React.InputHTMLAttributes<HTMLDivElement>;

const JQueryUIDialog = ({
	autoOpen,
	buttons,
	children,
	close,
	modal,
	title,
	width,
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
				position: 'fixed',
				top: 0,
				left: 0,
				width: '100%',
				height: '100%',
			}}
			restoreFocusOptions={{ preventScroll: true }}
		>
			<>
				<div className="ui-widget-overlay ui-front" />
				<div
					{...props}
					className="ui-dialog ui-widget ui-widget-content ui-corner-all ui-front ui-dialog-buttons"
					role="dialog"
					style={{
						width: width,
						margin: '10vh auto',
						top: 0,
						left: '50%',
						transform: 'translateX(-50%)',
					}}
				>
					<div className="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix">
						<span className="ui-dialog-title">{title}</span>
						<button
							type="button"
							className="ui-button ui-widget ui-state-default ui-corner-all ui-button-icon-only ui-dialog-titlebar-close"
							title="close" /* TODO: localize */
							onClick={close}
						>
							<span className="ui-button-icon-primary ui-icon ui-icon-closethick" />
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
						className="ui-dialog-content ui-widget-content"
					>
						{children}
					</div>
					<div className="ui-dialog-buttonpane ui-widget-content ui-helper-clearfix">
						<div className="ui-dialog-buttonset">
							{buttons.map((button, index) => (
								<JQueryUIButton as="button" onClick={button.click} key={index}>
									{button.text}
								</JQueryUIButton>
							))}
						</div>
					</div>
				</div>
			</>
		</Modal>
	);
};

export default JQueryUIDialog;
