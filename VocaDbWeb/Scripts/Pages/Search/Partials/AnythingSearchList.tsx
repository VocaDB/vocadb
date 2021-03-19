import React, { Fragment, ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import { EntryUrlMapper } from '../../../../wwwroot/Scripts/App';
import EntryContract from '../../../../wwwroot/Scripts/DataContracts/EntryContract';
import PartialFindResultContract from '../../../../wwwroot/Scripts/DataContracts/PartialFindResultContract';
import EntryType from '../../../../wwwroot/Scripts/Models/EntryType';
import SafeAnchor from '../../../Bootstrap/SafeAnchor';
import ArtistTypeLabel from '../../../Shared/Partials/Artist/ArtistTypeLabel';
import DraftIcon from '../../../Shared/Partials/DraftIcon';
import EntryCountBox from '../../../Shared/Partials/EntryCountBox';
import ServerSidePaging from '../../../Shared/Partials/ServerSidePaging';
import SongTypeLabel from '../../../Shared/Partials/Song/SongTypeLabel';

interface AnythingSearchListProps {
	onPageChange: (page: number) => void;
	onPageSizeChange: (pageSize: number) => void;
	page: number;
	pageSize: number;
	result: PartialFindResultContract<EntryContract>;
}

const AnythingSearchList = ({
	onPageChange,
	onPageSizeChange,
	page,
	pageSize,
	result,
}: AnythingSearchListProps): ReactElement => {
	const { t } = useTranslation([
		'ViewRes',
		'ViewRes.Search',
		'VocaDb.Model.Resources',
		'VocaDb.Model.Resources.Albums',
		'VocaDb.Model.Resources.Songs',
		'VocaDb.Web.Resources.Domain',
		'VocaDb.Web.Resources.Domain.ReleaseEvents',
	]);

	const entryCategoryName = (entry: EntryContract): string => {
		switch (EntryType[entry.entryType]) {
			case EntryType.Artist:
				return t(`VocaDb.Model.Resources:ArtistTypeNames.${entry.artistType}`);
			case EntryType.Album:
				return t(`VocaDb.Model.Resources.Albums:DiscTypeNames.${entry.discType}`);
			case EntryType.ReleaseEvent:
				return t(`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${entry.eventCategory}`);
			case EntryType.Song:
				return t(`VocaDb.Model.Resources.Songs:SongTypeNames.${entry.songType}`);
			default:
				return null;
		}
	};

	const entryUrl = (entry: EntryContract): string => EntryUrlMapper.details(entry.entryType, entry.id);

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
							{t('ViewRes:Shared.Name')}
						</th>
						<th>
							{t('ViewRes.Search:Index.EntryType')}
						</th>
						<th /* TODO */>
							{t('ViewRes:Shared.Tags')}
						</th>
					</tr>
				</thead>
				<tbody>
					{result.items.map(item => (
						<tr key={`${item.entryType}.${item.id}`/* REVIEW: React */}>
							<td style={{ width: '80px' }}>
								{item.mainPicture?.urlTinyThumb && (
									<a href={entryUrl(item)} title={item.additionalNames} className="coverPicThumb">
										<img src={item.mainPicture.urlTinyThumb} title="Cover picture" className="coverPicThumb img-rounded" referrerPolicy="same-origin" />
									</a>
								)}
							</td>
							<td>
								<a href={entryUrl(item)} title={item.additionalNames}>{item.name}</a>
								{' '}
								<ArtistTypeLabel artistType={item.artistType} />
								<SongTypeLabel songType={item.songType} />
								{' '}
								<DraftIcon status={item.status} />
								{item.artistString && (
									<Fragment>
										<br />
										<small className="extraInfo">{item.artistString}</small>
									</Fragment>
								)}
							</td>
							<td>
								<SafeAnchor /* TODO */>{t(`VocaDb.Web.Resources.Domain:EntryTypeNames.${item.entryType}`)}</SafeAnchor>
								{(item.artistType || item.discType || item.songType || item.eventCategory) && (
									<Fragment>
										{' '}
										<small className="extraInfo">({entryCategoryName(item)})</small>
									</Fragment>
								)}
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

export default AnythingSearchList;
