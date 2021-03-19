import React, { Fragment, ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import { EntryUrlMapper } from '../../../../wwwroot/Scripts/App';
import PartialFindResultContract from '../../../../wwwroot/Scripts/DataContracts/PartialFindResultContract';
import SongApiContract from '../../../../wwwroot/Scripts/DataContracts/Song/SongApiContract';
import SafeAnchor from '../../../Bootstrap/SafeAnchor';
import DraftIcon from '../../../Shared/Partials/DraftIcon';
import EntryCountBox from '../../../Shared/Partials/EntryCountBox';
import ServerSidePaging from '../../../Shared/Partials/ServerSidePaging';
import SongTypeLabel from '../../../Shared/Partials/Song/SongTypeLabel';

interface SongSearchListProps {
	onPageChange: (page: number) => void;
	onPageSizeChange: (pageSize: number) => void;
	page: number;
	pageSize: number;
	result: PartialFindResultContract<SongApiContract>;
}

const SongSearchList = ({
	onPageChange,
	onPageSizeChange,
	page,
	pageSize,
	result,
}: SongSearchListProps): ReactElement => {
	const { t } = useTranslation(['ViewRes', 'ViewRes.Search']);

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
								{t('ViewRes.Search:Index.Rating')}
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
								{item.thumbUrl && (
									<a href={EntryUrlMapper.details_song(item)} title={item.additionalNames}>
										<img src={item.thumbUrl} title="Cover picture" className="coverPicThumb img-rounded" referrerPolicy="same-origin" />
									</a>
								)}
							</td>
							<td>
								{/* TODO */}

								<a href={EntryUrlMapper.details_song(item)} title={item.additionalNames}>{item.name}</a>
								{' '}
								<SongTypeLabel songType={item.songType} />

								{/* TODO */}

								{' '}
								<DraftIcon status={item.status} />

								{/* TODO */}

								<br />
								<small className="extraInfo">{item.artistString}</small>

								{/* TODO */}
							</td>
							<td /* TODO */ className="search-tags-column">
								{item.tags?.length > 0 && (
									<div>
										<i className="icon icon-tags fix-icon-margin"></i>
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
								<span>{item.ratingScore}</span>
								{' '}
								{t('ViewRes.Search:Index.TotalScore')}
							</td>
						</tr>
					))}
				</tbody>
			</table>

			<div /* TODO */>
				<ServerSidePaging onPageChange={onPageChange} page={page} pageSize={pageSize} totalItems={result.totalCount} />
			</div>

			{/* TODO */}
		</div>
	);
};

export default SongSearchList;
