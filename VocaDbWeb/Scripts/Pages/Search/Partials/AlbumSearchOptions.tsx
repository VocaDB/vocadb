import React, { ReactElement } from 'react';
import { useTranslation } from 'react-i18next';

const AlbumSearchOptions = (): ReactElement => {
	const { t } = useTranslation(['ViewRes.Search', 'VocaDb.Web.Resources.Domain']);

	return (
		<div>
			<div className="control-group">
				<div className="control-label">{t('ViewRes.Search:Index.AlbumType')}</div>
				<div className="controls">
					{/* TODO */}
				</div>
			</div>

			<div className="control-group">
				<div className="control-label">{t('VocaDb.Web.Resources.Domain:EntryTypeNames.Artist')}</div>
				<div className="controls" /* TODO */>
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

export default AlbumSearchOptions;
