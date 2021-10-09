import SafeAnchor from '@Bootstrap/SafeAnchor';
import EntryCountBox from '@Components/Shared/Partials/EntryCountBox';
import ServerSidePaging from '@Components/Shared/Partials/Knockout/ServerSidePaging';
import DraftIcon from '@Components/Shared/Partials/Shared/DraftIcon';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import EventSearchStore, {
	EventSortRule,
} from '@Stores/Search/EventSearchStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

const useCategoryName = (): ((event: ReleaseEventContract) => string) => {
	const { t } = useTranslation(['VocaDb.Web.Resources.Domain.ReleaseEvents']);

	return React.useCallback(
		(event: ReleaseEventContract): string => {
			var inherited = event.series ? event.series.category : event.category;

			if (!inherited || inherited === 'Unspecified') return '';

			return t(
				`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${inherited}`,
			);
		},
		[t],
	);
};

interface EventSearchListProps {
	eventSearchStore: EventSearchStore;
}

const EventSearchList = observer(
	({ eventSearchStore }: EventSearchListProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Event']);

		const categoryName = useCategoryName();

		return (
			<>
				<EntryCountBox pagingStore={eventSearchStore.paging} />

				<ServerSidePaging pagingStore={eventSearchStore.paging} />

				<table
					className={classNames(
						'table',
						'table-striped',
						eventSearchStore.loading && 'loading',
					)}
				>
					<thead>
						<tr>
							<th colSpan={2}>
								<SafeAnchor
									onClick={(): void =>
										runInAction(() => {
											eventSearchStore.sort = EventSortRule.Name;
										})
									}
								>
									{t('ViewRes:Shared.Name')}
									{eventSearchStore.sort === EventSortRule.Name && (
										<>
											{' '}
											<span className="sortDirection_down" />
										</>
									)}
								</SafeAnchor>
							</th>
							{eventSearchStore.showTags && <th>{t('ViewRes:Shared.Tags')}</th>}
							<th>
								<SafeAnchor
									onClick={(): void =>
										runInAction(() => {
											eventSearchStore.sort = EventSortRule.Date;
										})
									}
								>
									{t('ViewRes.Event:Details.OccurrenceDate')}
									{eventSearchStore.sort === EventSortRule.Date && (
										<>
											{' '}
											<span className="sortDirection_down" />
										</>
									)}
								</SafeAnchor>
							</th>
							<th>
								<SafeAnchor
									onClick={(): void =>
										runInAction(() => {
											eventSearchStore.sort = EventSortRule.SeriesName;
										})
									}
								>
									{t('ViewRes.Event:Details.Series')}
									{eventSearchStore.sort === EventSortRule.SeriesName && (
										<>
											{' '}
											<span className="sortDirection_down" />
										</>
									)}
								</SafeAnchor>
							</th>
							<th>
								<SafeAnchor
									onClick={(): void =>
										runInAction(() => {
											eventSearchStore.sort = EventSortRule.VenueName;
										})
									}
								>
									{t('ViewRes.Event:Details.Venue')}
									{eventSearchStore.sort === EventSortRule.VenueName && (
										<>
											{' '}
											<span className="sortDirection_down" />
										</>
									)}
								</SafeAnchor>
							</th>
						</tr>
					</thead>
					<tbody>
						{eventSearchStore.page.map((event) => (
							<tr key={event.id}>
								<td style={{ width: '80px' }}>
									{event.mainPicture && event.mainPicture.urlSmallThumb && (
										<a
											href={EntryUrlMapper.details(
												EntryType.ReleaseEvent,
												event.id,
												event.urlSlug,
											)}
											title={event.additionalNames}
										>
											{/* eslint-disable-next-line jsx-a11y/alt-text */}
											<img
												src={event.mainPicture.urlSmallThumb}
												title="Cover picture" /* TODO: localize */
												className="coverPicThumb img-rounded"
												referrerPolicy="same-origin"
											/>
										</a>
									)}
								</td>
								<td>
									<a
										href={EntryUrlMapper.details(
											EntryType.ReleaseEvent,
											event.id,
											event.urlSlug,
										)}
										title={event.additionalNames}
									>
										{event.name}
									</a>{' '}
									<DraftIcon
										status={
											EntryStatus[event.status as keyof typeof EntryStatus]
										}
									/>
									<br />
									<small className="extraInfo">{categoryName(event)}</small>
								</td>
								{eventSearchStore.showTags && (
									<td className="search-tags-column">
										{event.tags && event.tags.length > 0 && (
											<div>
												<i className="icon icon-tags" />{' '}
												{event.tags.map((tag, index) => (
													<React.Fragment key={tag.tag.id}>
														{index > 0 && ', '}
														<SafeAnchor
															onClick={(): void =>
																eventSearchStore.selectTag(tag.tag)
															}
														>
															{tag.tag.name}
														</SafeAnchor>
													</React.Fragment>
												))}
											</div>
										)}
									</td>
								)}
								<td>
									{event.date && (
										<span>{eventSearchStore.formatDate(event.date)}</span>
									)}
								</td>
								<td>
									{event.series && (
										<a
											href={EntryUrlMapper.details(
												EntryType.ReleaseEventSeries,
												event.series.id,
												event.series.urlSlug,
											)}
										>
											{event.series.name}
										</a>
									)}
								</td>
								<td>
									{event.venue ? (
										<a
											href={EntryUrlMapper.details(
												EntryType.Venue,
												event.venue.id,
											)}
										>
											{event.venue.name}
										</a>
									) : (
										event.venueName && <span>{event.venueName}</span>
									)}
								</td>
							</tr>
						))}
					</tbody>
				</table>

				<ServerSidePaging pagingStore={eventSearchStore.paging} />
			</>
		);
	},
);

export default EventSearchList;
