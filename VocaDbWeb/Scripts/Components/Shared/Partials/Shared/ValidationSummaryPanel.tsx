import Alert from '@/Bootstrap/Alert';
import _ from 'lodash';
import React from 'react';

interface ValidationSummaryPanelProps {
	message: string;
	errors: Record<string, string[]>;
}

const ValidationSummaryPanel = React.memo(
	({ message, errors }: ValidationSummaryPanelProps): React.ReactElement => {
		const errorTexts = React.useMemo(
			() => _.chain(errors).values().flatMap().value(),
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

export default ValidationSummaryPanel;
