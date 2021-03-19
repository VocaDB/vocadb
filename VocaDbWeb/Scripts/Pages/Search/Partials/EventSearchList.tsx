import React, { Fragment, ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import { EntryUrlMapper } from '../../../../wwwroot/Scripts/App';
import PartialFindResultContract from '../../../../wwwroot/Scripts/DataContracts/PartialFindResultContract';
import ReleaseEventContract from '../../../../wwwroot/Scripts/DataContracts/ReleaseEvents/ReleaseEventContract';
import EntryType from '../../../../wwwroot/Scripts/Models/EntryType';
import SafeAnchor from '../../../Bootstrap/SafeAnchor';
import DraftIcon from '../../../Shared/Partials/DraftIcon';
import EntryCountBox from '../../../Shared/Partials/EntryCountBox';
import ServerSidePaging from '../../../Shared/Partials/ServerSidePaging';

interface EventSearchListProps {
	onPageChange: (page: number) => void;
	onPageSizeChange: (pageSize: number) => void;
	page: number;
	pageSize: number;
	result: PartialFindResultContract<ReleaseEventContract>;
}

const EventSearchList = ({
	onPageChange,
	onPageSizeChange,
	page,
	pageSize,
	result,
}: EventSearchListProps): ReactElement => {
	const { t } = useTranslation(['ViewRes', 'ViewRes.Event', 'VocaDb.Web.Resources.Domain.ReleaseEvents']);

	const formatDate = (dateStr: string): string => moment(dateStr).utc().format('l');

	const getCategoryName = (event: ReleaseEventContract): string => {
		var inherited = event.series ? event.series.category : event.category;

		if (!inherited || inherited === 'Unspecified'/* TODO: enum */)
			return '';

			return t(`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${inherited}`);
	};

	return (
		<div>
			<EntryCountBox onPageSizeChange={onPageSizeChange} pageSize={pageSize} totalItems={result.totalCount} />

			<div /* TODO */>
				<ServerSidePaging onPageChange={onPageChange} page={page} pageSize={pageSize} totalItems={result.totalCount} />
			</div>

			<table className="table table-striped" /* TODO */>
				<thead>
					<tr>
						<th colSpan={2}>
							<a /* TODO */ href="#">
								{t('ViewRes:Shared.Name')}
								{' '}
								{/* TODO */}
							</a>
						</th>
						<th /* TODO */>
							{t('ViewRes:Shared.Tags')}
						</th>
						<th>
							<a /* TODO */ href="#">
								{t('ViewRes.Event:Details.OccurrenceDate')}
								{' '}
								{/* TODO */}
							</a>
						</th>
						<th>
							<a /* TODO */ href="#">
								{t('ViewRes.Event:Details.Series')}
								{' '}
								{/* TODO */}
							</a>
						</th>
						<th>
							<a /* TODO */ href="#">
								{t('ViewRes.Event:Details.Venue')}
								{' '}
								{/* TODO */}
							</a>
						</th>
					</tr>
				</thead>
				<tbody>
					{result.items.map(item => (
						<tr key={item.id}>
							<td style={{ width: '80px' }}>
								{item.mainPicture?.urlSmallThumb && (
									<a href={EntryUrlMapper.details(EntryType.ReleaseEvent, item.id, item.urlSlug)} title={item.additionalNames}>
										<img src={item.mainPicture.urlSmallThumb} title="Cover picture" className="coverPicThumb img-rounded" referrerPolicy="same-origin" />
									</a>
								)}
							</td>
							<td>
								<a href={EntryUrlMapper.details(EntryType.ReleaseEvent, item.id, item.urlSlug)} title={item.additionalNames}>{item.name}</a>

								{' '}
								<DraftIcon status={item.status} />

								<br />
								<small className="extraInfo">{getCategoryName(item)}</small>
							</td>
							<td /* TODO */ className="search-tags-column">
								{item.tags?.length > 0 && (
									<div>
										<i className="icon icon-tags"></i>
										{' '}
										{item.tags.map(tag => (
											<Fragment key={tag.tag.id}>
												<SafeAnchor /* TODO */>{tag.tag.name}</SafeAnchor>
												{tag != _.last(item.tags) && (
													<Fragment>
														<span>,</span>
														{' '}
													</Fragment>
												)}
											</Fragment>
										))}
									</div>
								)}
							</td>
							<td>
								{item.date && (
									<span>{formatDate(item.date)}</span>
								)}
							</td>
							<td>
								{item.series && (
									<a href={EntryUrlMapper.details(EntryType.ReleaseEvent, item.series.id, item.series.urlSlug)}>{item.series.name}</a>
								)}
							</td>
							<td>
								{item.venue && (
									<a href={EntryUrlMapper.details(EntryType.Venue, item.venue.id)}>{item.venue.name}</a>
								)}
								{(!item.venue && item.venueName) && (
									<span>{item.venueName}</span>
								)}
							</td>
						</tr>
					))}
				</tbody>
			</table>

			<div /* TODO */>
				<ServerSidePaging onPageChange={onPageChange} page={page} pageSize={pageSize} totalItems={result.totalCount} />
			</div>
		</div>
	);
};

export default EventSearchList;
