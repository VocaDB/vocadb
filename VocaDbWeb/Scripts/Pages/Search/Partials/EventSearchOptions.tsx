import Button from '@/Bootstrap/Button';
import { ArtistFilters } from '@/Components/Shared/Partials/Knockout/ArtistFilters';
import { ReleaseEventCategoryDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import JQueryUIDatepicker from '@/JQueryUI/JQueryUIDatepicker';
import { EventSearchStore } from '@/Stores/Search/EventSearchStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EventSearchOptionsProps {
	eventSearchStore: EventSearchStore;
}

const EventSearchOptions = observer(
	({ eventSearchStore }: EventSearchOptionsProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes.Event',
			'ViewRes.Search',
			'VocaDb.Web.Resources.Domain',
		]);

		return (
			<div>
				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.Search:Index.EventCategory')}
					</div>
					<div className="controls">
						<div className="input-append">
							<ReleaseEventCategoryDropdownList
								value={eventSearchStore.category}
								onChange={(e): void =>
									runInAction(() => {
										eventSearchStore.category = e.target.value;
									})
								}
							/>
						</div>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">
						{t('VocaDb.Web.Resources.Domain:EntryTypeNames.Artist')}
					</div>
					<div className="controls">
						<ArtistFilters
							artistFilters={eventSearchStore.artistFilters}
							artistParticipationStatus={false}
						/>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.Event:Details.OccurrenceDate')}
					</div>
					<div className="controls">
						<JQueryUIDatepicker
							type="text"
							value={eventSearchStore.afterDate}
							onSelect={(date): void =>
								runInAction(() => {
									eventSearchStore.afterDate = date;
								})
							}
							dateFormat="yy-mm-dd"
							className="input-small"
							maxLength={10}
							placeholder="Begin" /* LOC */
						/>{' '}
						-{' '}
						<JQueryUIDatepicker
							type="text"
							value={eventSearchStore.beforeDate}
							onSelect={(date): void =>
								runInAction(() => {
									eventSearchStore.beforeDate = date;
								})
							}
							dateFormat="yy-mm-dd"
							className="input-small"
							maxLength={10}
							placeholder="End" /* LOC */
						/>{' '}
						<Button
							onClick={(): void =>
								runInAction(() => {
									eventSearchStore.afterDate = new Date();
								})
							}
						>
							{t('ViewRes.Search:Index.Today')}
						</Button>
					</div>
				</div>
			</div>
		);
	},
);

export default EventSearchOptions;
