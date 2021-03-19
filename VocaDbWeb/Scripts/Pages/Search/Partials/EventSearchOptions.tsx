import React, { ReactElement } from 'react';
import { useTranslation } from 'react-i18next';

const EventSearchOptions = (): ReactElement => {
	const { t } = useTranslation(['ViewRes.Search', 'VocaDb.Web.Resources.Domain', 'ViewRes.Event']);

	return (
		<div>
			<div className="control-group">
				<div className="control-label">{t('ViewRes.Search:Index.EventCategory')}</div>
				<div className="input-append">
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
				<div className="control-label">{t('ViewRes.Event:Details.OccurrenceDate')}</div>
				<div className="controls">
					{/* TODO */}
				</div>
			</div>
		</div>
	);
};

export default EventSearchOptions;
