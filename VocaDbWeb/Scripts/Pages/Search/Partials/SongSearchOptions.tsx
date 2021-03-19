import React, { ReactElement } from 'react';
import { useTranslation } from 'react-i18next';

const SongSearchOptions = (): ReactElement => {
	const { t } = useTranslation(['ViewRes.Search', 'VocaDb.Web.Resources.Domain']);

	return (
		<div>
			<div className="control-group">
				<div className="control-label">{t('ViewRes.Search:Index.SongType')}</div>
				<div className="controls">
					<div className="control-group">
						{/* TODO */}
					</div>
					<div className="control-group" /* TODO */>
						{/* TODO */}
					</div>
				</div>
			</div>

			<div className="control-group">
				<div className="control-label">{t('VocaDb.Web.Resources.Domain:EntryTypeNames.Artist')}</div>
				<div className="control" /* TODO */>
					{/* TODO */}
				</div>
			</div>

			<div className="control-group">
				<div className="control-label">{t('ViewRes.Search:Index.MoreRecentThan')}</div>
				<div className="controls">
					<select /* TODO */>
						<option value="">(Show all)</option>
						<option value="24">1 day</option>
						<option value="48">2 days</option>
						<option value="168">7 days</option>
						<option value="336">2 weeks</option>
						<option value="720">1 month</option>
						<option value="4320">6 months</option>
						<option value="8760">1 year</option>
						<option value="17520">2 years</option>
						<option value="26280">3 years</option>
					</select>
				</div>
			</div>

			<div className="control-group">
				<div className="control-label">{t('ViewRes.Search:Index.MinScore')}</div>
				<div className="controls">
					<input type="number" /* TODO */ maxLength={10} min={0} className="input-small" />
				</div>
			</div>

			<div className="control-group">
				<div className="control-label">{t('ViewRes.Search:Index.ReleaseEvent')}</div>
				<div className="controls">
					{/* TODO */}
				</div>
			</div>

			<div className="control-group">
				<div className="control-label">{t('ViewRes.Search:Index.ReleaseDate')}</div>
				<div className="controls">
					<input type="number" /* TODO */ className="input-small" maxLength={4} max={2100} min={1900} placeholder="Year" />
					{' '}
					<input type="number" /* TODO */ className="input-small" maxLength={2} max={12} min={1} placeholder="Month" />
				</div>
			</div>

			<div className="control-group">
				<div className="control-label">{t('ViewRes.Search:Index.ParentVersion')}</div>
				<div className="controls">
					{/* TODO */}
				</div>
			</div>

			<div className="control-group">
				<div className="control-label">{t('ViewRes.Search:Index.Duration')}</div>
				<div className="controls">
					<input type="text" /* TODO */ className="input-small" maxLength={10} />
					{' '}
					-
					{' '}
					<input type="text" /* TODO */ className="input-small" maxLength={10} />
				</div>
			</div>

			<div className="control-group">
				<div className="control-label">{t('ViewRes.Search:Index.Bpm')}</div>
				<div className="controls">
					<input type="number" /* TODO */ className="input-small" maxLength={10} min={20} max={400} step={0.01} />
					{' '}
					-
					{' '}
					<input type="number" /* TODO */ className="input-small" maxLength={10} min={20} max={400} step={0.01} />
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

export default SongSearchOptions;
