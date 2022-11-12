import Alert from '@/Bootstrap/Alert';
import Button from '@/Bootstrap/Button';
import classNames from 'classnames';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface EntryValidationMessageProps {
	draft: boolean;
	validationMessages: string[];
	helpSection?: string;
}

export const EntryValidationMessage = React.memo(
	({
		draft,
		validationMessages,
		helpSection,
	}: EntryValidationMessageProps): React.ReactElement => {
		const { t } = useTranslation(['HelperRes']);

		const [validationExpanded, setValidationExpanded] = React.useState(false);

		return (
			<>
				{draft
					? validationMessages.length === 0 && (
							<Alert variant="success" className="alert-no-bottom-margin">
								<span className="icon-line tickIcon" />{' '}
								{t('HelperRes:Helper.EntryValidationNoErrorsDraft')}
							</Alert>
					  )
					: validationMessages.length === 0 && (
							<Alert variant="success" className="alert-no-bottom-margin">
								<span className="icon-line tickIcon" />{' '}
								{t('HelperRes:Helper.EntryValidationNoErrors')}
							</Alert>
					  )}

				{validationMessages.length > 0 && (
					<Alert className="alert-no-bottom-margin">
						<div className="pull-right">
							<Button
								onClick={(): void => setValidationExpanded(!validationExpanded)}
								className={classNames(
									'btn-mini',
									validationExpanded && 'active',
								)}
								href="#"
							>
								<i className="icon-plus noMargin" />{' '}
								{t('HelperRes:Helper.EntryValidationWarningDetails')}
							</Button>
						</div>

						{validationExpanded ? (
							<>
								<h4>{t('HelperRes:Helper.EntryIsMissingInformation')}</h4>
								<ul className="entry-validation-list">
									{validationMessages.map((validationMessage, index) => (
										<li key={index}>{validationMessage}</li>
									))}
								</ul>
								{helpSection && (
									<span>
										See the{' '}
										<Link to={`/Help/guidelines#${helpSection}`}>guide</Link>{' '}
										for more information.
										{/* LOC */}
									</span>
								)}
							</>
						) : (
							<div className="entry-validation-error-summary">
								<span className="icon-line errorIcon" />{' '}
								<strong>
									{t('HelperRes:Helper.EntryIsMissingInformation')}
								</strong>{' '}
								{validationMessages.map((validationMessage, index) => (
									<React.Fragment key={index}>
										{index > 0 && ' '}
										<span>{validationMessage}</span>
									</React.Fragment>
								))}
							</div>
						)}
					</Alert>
				)}
			</>
		);
	},
);
