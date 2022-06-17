import Button from '@Bootstrap/Button';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface SaveAndBackBtnProps {
	backAction: string;
	submitting?: boolean;
}

const SaveAndBackBtn = React.memo(
	({ backAction, submitting }: SaveAndBackBtnProps): React.ReactElement => {
		const { t } = useTranslation('HelperRes');

		return (
			<p>
				<Button type="submit" variant="primary" disabled={submitting}>
					<i className="icon-ok icon-white"></i> &nbsp;
					{t('HelperRes:Helper.SaveChanges')}
				</Button>{' '}
				<Button className="btn-nomargin" as={Link} to={backAction}>
					<i className="icon-backward"></i> &nbsp;
					{t('HelperRes:Helper.ReturnWithoutSaving')}
				</Button>
			</p>
		);
	},
);

export default SaveAndBackBtn;
