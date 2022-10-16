import Alert from '@/Bootstrap/Alert';
import React from 'react';

interface ValidationSummaryPanelProps {
	message: string;
	errors: Record<string, string[]>;
}

export const ValidationSummaryPanel = React.memo(
	({ message, errors }: ValidationSummaryPanelProps): React.ReactElement => {
		const errorTexts = React.useMemo(
			() => Object.entries(errors).map(([, value]) => value[0]),
			[errors],
		);

		return (
			<Alert variant="error">
				<h4>{message}</h4>
				<div className="validation-summary-errors">
					<ul>
						{errorTexts.map((errorText, index) => (
							<li key={index}>{errorText}</li>
						))}
					</ul>
				</div>
			</Alert>
		);
	},
);
