import Button from '@/Bootstrap/Button';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SaveBtnProps {
	submitting: boolean;
}

export const SaveBtn = React.memo(
	({ submitting }: SaveBtnProps): React.ReactElement => {
		const { t } = useTranslation('HelperRes');

		return (
			<p>
				<Button type="submit" variant="primary" disabled={submitting}>
					<i className="icon-ok icon-white"></i> &nbsp;
					{t('HelperRes:Helper.SaveChanges')}
				</Button>
			</p>
		);
	},
);
