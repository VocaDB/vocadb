import React, { ReactElement } from 'react';
import { useTranslation } from 'react-i18next';

const ArtistSearchOptions = (): ReactElement => {
	const { t } = useTranslation(['ViewRes.Search']);

	return (
		<div>
			<div className="control-group">
				<div className="control-label">{t('ViewRes.Search:Index.ArtistType')}</div>
				<div className="controls">
					{/* TODO */}
				</div>
			</div>

			<div className="control-group">
				<div className="control-label"></div>
				<div className="controls">
					{/* TODO */}
				</div>
			</div>
		</div>
	);
};

export default ArtistSearchOptions;
