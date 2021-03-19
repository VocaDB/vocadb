import React, { Fragment, ReactElement } from 'react';
import { useTranslation } from 'react-i18next';

interface DraftIconProps {
	status: string;
}

const DraftIcon = ({
	status,
}: DraftIconProps): ReactElement => {
	const { t } = useTranslation(['HelperRes']);

	return (
		<Fragment>
			{status === 'Draft'/* TODO: enum */ && (
				<img src={'/Content/draft.png'/* REVIEW: React */} title={t('HelperRes:Helper.DraftIconTitle')} alt="draft" />
			)}
		</Fragment>
	);
};

export default DraftIcon;
