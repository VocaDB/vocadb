import React, { Fragment, ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import { EntryUrlMapper } from '../../../../wwwroot/Scripts/App';
import AlbumContract from '../../../../wwwroot/Scripts/DataContracts/Album/AlbumContract';
import AlbumForApiContract from '../../../../wwwroot/Scripts/DataContracts/Album/AlbumForApiContract';
import PartialFindResultContract from '../../../../wwwroot/Scripts/DataContracts/PartialFindResultContract';
import EntryType from '../../../../wwwroot/Scripts/Models/EntryType';
import SafeAnchor from '../../../Bootstrap/SafeAnchor';
import DraftIcon from '../../../Shared/Partials/DraftIcon';
import EntryCountBox from '../../../Shared/Partials/EntryCountBox';
import ServerSidePaging from '../../../Shared/Partials/ServerSidePaging';

interface AlbumSearchListProps {
	onPageChange: (page: number) => void;
	onPageSizeChange: (pageSize: number) => void;
	page: number;
	pageSize: number;
	result: PartialFindResultContract<AlbumForApiContract>;
	unknownPictureUrl: string;
}

const AlbumSearchList = ({
	onPageChange,
	onPageSizeChange,
	page,
	pageSize,
	result,
	unknownPictureUrl,
}: AlbumSearchListProps): ReactElement => {
	const { t } = useTranslation(['ViewRes', 'ViewRes.Search', 'VocaDb.Model.Resources.Albums']);

	const discTypeName = (discTypeStr: string): string => t(`VocaDb.Model.Resources.Albums:DiscTypeNames.${discTypeStr}`);

	const ratingStars = (album: AlbumContract): { enabled: boolean }[] => {
		if (!album)
			return [];

		var ratings = _.map([1, 2, 3, 4, 5], rating => {
			return {
				enabled: (Math.round(album.ratingAverage) >= rating),
			}
		});
		return ratings;
	}

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
								{t('ViewRes.Search:Index.ReleaseDate')}
								{' '}
								{/* TODO */}
							</a>
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
								<a href={EntryUrlMapper.details(EntryType.Album, item.id)} title={item.additionalNames} className="coverPicThumb">
									<img src={item.mainPicture?.urlTinyThumb ?? unknownPictureUrl} title="Cover picture" alt="Cover picture" className="coverPicThumb img-rounded" />
								</a>
							</td>
							<td>
								<a href={EntryUrlMapper.details(EntryType.Album, item.id)} title={item.additionalNames}>{item.name/* TODO */}</a>
								{' '}
								<DraftIcon status={item.status} />
								<br />
								<small className="extraInfo">{item.artistString}</small><br />
								<small className="extraInfo">{discTypeName(item.discType)}</small>
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
							<td className="search-album-release-date-column">
								{item.releaseDate && (
									<span>{item.releaseDate.formatted}</span>
								)}
								{item.releaseEvent && (
									<span>
										<br />
										<a href={EntryUrlMapper.details(EntryType.ReleaseEvent, item.releaseEvent.id)}>{item.releaseEvent.name}</a>
									</span>
								)}
							</td>
							<td style={{ width: '150px' }}>
								<span /* TODO */>
									{ratingStars(item).map((ratingStar, i) => (
										<img src={ratingStar.enabled ? '/Content/star.png' : '/Content/star_disabled.png'} key={i} />
									))}
								</span>
								<br />
								<span>{item.ratingCount}</span>
								{' '}
								{t('ViewRes.Search:Index.Times')}
							</td>
						</tr>
					))}
				</tbody>
			</table>

			{/* TODO */}

			<div /* TODO */>
				<ServerSidePaging onPageChange={onPageChange} page={page} pageSize={pageSize} totalItems={result.totalCount} />
			</div>
		</div>
	);
};

export default AlbumSearchList;
