import ArtistFilters from '@Components/Shared/Partials/Knockout/ArtistFilters';
import ReleaseEventLockingAutoComplete from '@Components/Shared/Partials/Knockout/ReleaseEventLockingAutoComplete';
import SongLockingAutoComplete from '@Components/Shared/Partials/Knockout/SongLockingAutoComplete';
import { SongAdvancedFilters } from '@Components/Shared/Partials/Search/AdvancedFilters';
import SongTypesDropdownKnockout from '@Components/Shared/Partials/Song/SongTypesDropdownKnockout';
import { useRedial } from '@Components/redial';
import SongSearchStore from '@Stores/Search/SongSearchStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';

import SongBpmFilter from './SongBpmFilter';
import SongLengthFilter from './SongLengthFilter';

interface SongSearchOptionsProps {
	songSearchStore: SongSearchStore;
}

const SongSearchOptions = observer(
	({ songSearchStore }: SongSearchOptionsProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Search',
			'VocaDb.Web.Resources.Domain:EntryTypeNames',
		]);
		const redial = useRedial(songSearchStore.routeParams);

		return (
			<div>
				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.Search:Index.SongType')}
					</div>
					<div className="controls">
						<div className="control-group">
							<SongTypesDropdownKnockout
								activeKey={songSearchStore.songType}
								onSelect={(eventKey): void =>
									redial({ songType: eventKey, page: 1 })
								}
							/>
						</div>
						{songSearchStore.showUnifyEntryTypesAndTags && (
							<div className="control-group">
								<label className="checkbox">
									<input
										type="checkbox"
										checked={songSearchStore.unifyEntryTypesAndTags}
										onChange={(e): void =>
											runInAction(() => {
												songSearchStore.unifyEntryTypesAndTags =
													e.target.checked;
											})
										}
									/>
									Also search associated tag{/* TODO: localize */}
								</label>
							</div>
						)}
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">
						{t('VocaDb.Web.Resources.Domain:EntryTypeNames.Artist')}
					</div>
					<div className="controls">
						<ArtistFilters
							artistFilters={songSearchStore.artistFilters}
							artistParticipationStatus={true}
						/>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.Search:Index.MoreRecentThan')}
					</div>
					<div className="controls">
						<select
							value={songSearchStore.since}
							onChange={(e): void =>
								runInAction(() => {
									songSearchStore.since = Number(e.target.value);
								})
							}
						>
							<option value="">(Show all){/* TODO: localize */}</option>
							<option value="24">1 day{/* TODO: localize */}</option>
							<option value="48">2 days{/* TODO: localize */}</option>
							<option value="168">7 days{/* TODO: localize */}</option>
							<option value="336">2 weeks{/* TODO: localize */}</option>
							<option value="720">1 month{/* TODO: localize */}</option>
							<option value="4320">6 months{/* TODO: localize */}</option>
							<option value="8760">1 year{/* TODO: localize */}</option>
							<option value="17520">2 years{/* TODO: localize */}</option>
							<option value="26280">3 years{/* TODO: localize */}</option>
						</select>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.Search:Index.MinScore')}
					</div>
					<div className="controls">
						<DebounceInput
							type="number"
							value={songSearchStore.minScore ?? ''}
							onChange={(e): void =>
								runInAction(() => {
									songSearchStore.minScore = e.target.value
										? Number(e.target.value)
										: undefined;
								})
							}
							maxLength={10}
							min={0}
							className="input-small"
							debounceTimeout={300}
						/>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.Search:Index.ReleaseEvent')}
					</div>
					<div className="controls">
						<ReleaseEventLockingAutoComplete
							basicEntryLinkStore={songSearchStore.releaseEvent}
						/>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.Search:Index.ReleaseDate')}
					</div>
					<div className="controls">
						<DebounceInput
							type="number"
							value={songSearchStore.dateYear ?? ''}
							onChange={(e): void =>
								runInAction(() => {
									songSearchStore.dateYear = e.target.value
										? Number(e.target.value)
										: undefined;
								})
							}
							className="input-small"
							maxLength={4}
							max={2100}
							min={1900}
							placeholder="Year" /* TODO: localize */
							debounceTimeout={300}
						/>
						{songSearchStore.dateYear && (
							<>
								{' '}
								<DebounceInput
									type="number"
									value={songSearchStore.dateMonth ?? ''}
									onChange={(e): void =>
										runInAction(() => {
											songSearchStore.dateMonth = e.target.value
												? Number(e.target.value)
												: undefined;
										})
									}
									className="input-small"
									maxLength={2}
									max={12}
									min={1}
									placeholder="Month" /* TODO: localize */
									debounceTimeout={300}
								/>
							</>
						)}
						{songSearchStore.dateMonth && (
							<>
								{' '}
								<DebounceInput
									type="number"
									value={songSearchStore.dateDay ?? ''}
									onChange={(e): void => {
										runInAction(() => {
											songSearchStore.dateDay = e.target.value
												? Number(e.target.value)
												: undefined;
										});
									}}
									className="input-small"
									maxLength={2}
									max={31}
									min={1}
									placeholder="Day" /* TODO: localize */
									debounceTimeout={300}
								/>
							</>
						)}
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.Search:Index.ParentVersion')}
					</div>
					<div className="controls">
						<SongLockingAutoComplete
							basicEntryLinkStore={songSearchStore.parentVersion}
						/>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.Search:Index.Duration')}
					</div>
					<div className="controls">
						<SongLengthFilter
							songLengthFilter={songSearchStore.minLengthFilter}
						/>{' '}
						-{' '}
						<SongLengthFilter
							songLengthFilter={songSearchStore.maxLengthFilter}
						/>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">{t('ViewRes.Search:Index.Bpm')}</div>
					<div className="controls">
						<SongBpmFilter songBpmFilter={songSearchStore.minBpmFilter} /> -{' '}
						<SongBpmFilter songBpmFilter={songSearchStore.maxBpmFilter} />
					</div>
				</div>

				<div className="control-group">
					<div className="control-label"></div>
					<div className="controls">
						<SongAdvancedFilters
							advancedFilters={songSearchStore.advancedFilters}
						/>
					</div>
				</div>
			</div>
		);
	},
);

export default SongSearchOptions;
