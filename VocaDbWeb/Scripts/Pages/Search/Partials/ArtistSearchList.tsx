import React, { Fragment, ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import { EntryUrlMapper } from '../../../../wwwroot/Scripts/App';
import ArtistApiContract from '../../../../wwwroot/Scripts/DataContracts/Artist/ArtistApiContract';
import PartialFindResultContract from '../../../../wwwroot/Scripts/DataContracts/PartialFindResultContract';
import EntryType from '../../../../wwwroot/Scripts/Models/EntryType';
import SafeAnchor from '../../../Bootstrap/SafeAnchor';
import ArtistTypeLabel from '../../../Shared/Partials/Artist/ArtistTypeLabel';
import DraftIcon from '../../../Shared/Partials/DraftIcon';
import EntryCountBox from '../../../Shared/Partials/EntryCountBox';
import ServerSidePaging from '../../../Shared/Partials/ServerSidePaging';

interface ArtistSearchListProps {
	onPageChange: (page: number) => void;
	onPageSizeChange: (pageSize: number) => void;
	page: number;
	pageSize: number;
	result: PartialFindResultContract<ArtistApiContract>;
}

const ArtistSearchList = ({
	onPageChange,
	onPageSizeChange,
	page,
	pageSize,
	result,
}: ArtistSearchListProps): ReactElement => {
	const { t } = useTranslation(['ViewRes', 'ViewRes.Search', 'VocaDb.Model.Resources']);

	const artistTypeName = (artist: ArtistApiContract): string => t(`VocaDb.Model.Resources:ArtistTypeNames.${artist.artistType}`)/* REVIEW: React */;

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
								{t('ViewRes:Shared.ArtistName')}
								{' '}
								{/* TODO */}
							</a>
						</th>
						<th>{t('ViewRes.Search:Index.ArtistType')}</th>
						<th></th>
						<th /* TODO */>
							{t('ViewRes:Shared.Tags')}
						</th>
					</tr>
				</thead>
				<tbody>
					{result.items.map(item => (
						<tr key={item.id}>
							<td style={{ width: '80px' }}>
								{item.mainPicture?.urlTinyThumb && (
									<a href={EntryUrlMapper.details(EntryType.Artist, item.id)} title={item.additionalNames} className="coverPicThumb">
										<img src={item.mainPicture.urlTinyThumb} title="Cover picture" className="coverPicThumb img-rounded" referrerPolicy="same-origin" />
									</a>
								)}
							</td>
							<td>
								<a href={EntryUrlMapper.details(EntryType.Artist, item.id)}>{item.name}</a>
								{' '}
								<ArtistTypeLabel artistType={item.artistType} />
								{' '}
								<DraftIcon status={item.status} />
								<br />
								<small className="extraInfo">{item.additionalNames}</small>
							</td>
							<td>
								<span>{artistTypeName(item)}</span>
							</td>
							<td>
								{/* TODO */}
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

export default ArtistSearchList;
