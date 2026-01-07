import Button from '@/Bootstrap/Button';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SaveBtnProps {
	submitting: boolean;
	children?: React.ReactNode;
}

export const SaveBtn = React.memo(
	({ submitting, children }: SaveBtnProps): React.ReactElement => {
		const { t } = useTranslation('HelperRes');

		return (
			<p>
				<Button type="submit" variant="primary" disabled={submitting}>
					<i
						className={`${
							submitting ? 'icon-refresh icon-spin' : 'icon-ok'
						} icon-white`}
					></i>{' '}
					&nbsp;
					{children ?? t('HelperRes:Helper.SaveChanges')}
				</Button>
			</p>
		);
	},
);
