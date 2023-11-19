import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { EntryCountBox } from '@/Components/Shared/Partials/EntryCountBox';
import { ServerSidePaging } from '@/Components/Shared/Partials/Knockout/ServerSidePaging';
import { AlbumThumbItem } from '@/Components/Shared/Partials/Shared/AlbumThumbItem';
import { DraftIcon } from '@/Components/Shared/Partials/Shared/DraftIcon';
import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { AlbumSortRule } from '@/Stores/Search/AlbumSearchStore';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import classNames from 'classnames';
import { truncate } from 'lodash-es';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface AlbumSearchListTilesProps {
	albums: AlbumContract[];
}

export const AlbumSearchListTiles = React.memo(
	({ albums }: AlbumSearchListTilesProps): React.ReactElement => {
		return (
			<div className="smallThumbs">
				{albums.map((album) => (
					<AlbumThumbItem album={album} tooltip={true} key={album.id} />
				))}
			</div>
		);
	},
);

interface IAlbumSearchStore {
	loading: boolean;
	page: AlbumContract[];
	paging: ServerSidePagingStore;
	ratingStars?: (album: AlbumContract) => { enabled: boolean }[];
	selectTag?: (tag: TagBaseContract) => void;
	showTags: boolean;
	sort: AlbumSortRule;
	viewMode: string /* TODO: enum */;
}

interface AlbumSearchListProps {
	albumSearchStore: IAlbumSearchStore;
}

const AlbumSearchList = observer(
	({ albumSearchStore }: AlbumSearchListProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Search',
			'VocaDb.Model.Resources.Albums',
		]);

		return (
			<>
				<EntryCountBox pagingStore={albumSearchStore.paging} />

				<ServerSidePaging pagingStore={albumSearchStore.paging} />

				{albumSearchStore.viewMode === 'Details' && (
					<table
						className={classNames(
							'table',
							'table-striped',
							albumSearchStore.loading && 'loading',
						)}
					>
						<thead>
							<tr>
								<th colSpan={2}>
									<SafeAnchor
										onClick={(): void =>
											runInAction(() => {
												albumSearchStore.sort = AlbumSortRule.Name;
											})
										}
									>
										{t('ViewRes:Shared.Name')}
										{albumSearchStore.sort === AlbumSortRule.Name && (
											<>
												{' '}
												<span className="sortDirection_down" />
											</>
										)}
									</SafeAnchor>
								</th>
								{albumSearchStore.showTags && (
									<th>{t('ViewRes:Shared.Tags')}</th>
								)}
								<th>
									<SafeAnchor
										onClick={(): void =>
											runInAction(() => {
												albumSearchStore.sort = AlbumSortRule.ReleaseDate;
											})
										}
									>
										{t('ViewRes.Search:Index.ReleaseDate')}
										{albumSearchStore.sort === AlbumSortRule.ReleaseDate && (
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
												albumSearchStore.sort = AlbumSortRule.RatingAverage;
											})
										}
									>
										{t('ViewRes.Search:Index.Rating')}
										{albumSearchStore.sort === AlbumSortRule.RatingAverage && (
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
							{albumSearchStore.page.map((album) => (
								<tr key={album.id}>
									<td style={{ width: '80px' }}>
										<Link
											to={EntryUrlMapper.details(EntryType.Album, album.id)}
											title={album.additionalNames}
											className="coverPicThumb"
										>
											{/* eslint-disable-next-line jsx-a11y/img-redundant-alt */}
											<img
												src={
													album.mainPicture?.urlTinyThumb ??
													'/Content/unknown.png'
												}
												title="Cover picture" /* LOC */
												alt="Cover picture" /* LOC */
												className="coverPicThumb img-rounded"
											/>
										</Link>
									</td>
									<td>
										<Link
											to={EntryUrlMapper.details(EntryType.Album, album.id)}
											title={album.additionalNames}
										>
											{truncate(album.name, { length: 150 })}
										</Link>{' '}
										<DraftIcon status={album.status} />
										<br />
										<small className="extraInfo">{album.artistString}</small>
										<br />
										<small className="extraInfo">
											{t(
												`VocaDb.Model.Resources.Albums:DiscTypeNames.${album.discType}`,
											)}
										</small>
									</td>
									{albumSearchStore.showTags && (
										<td className="search-tags-column">
											{album.tags && album.tags.length > 0 && (
												<div>
													<i className="icon icon-tags fix-icon-margin" />{' '}
													{album.tags.map((tag, index) => (
														<React.Fragment key={tag.tag.id}>
															{index > 0 && ', '}
															<SafeAnchor
																onClick={(): void =>
																	albumSearchStore.selectTag?.(tag.tag)
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
									<td className="search-album-release-date-column">
										<span>
											{DateTimeHelper.formatComponentDate(
												album.releaseDate.year,
												album.releaseDate.month,
												album.releaseDate.day,
											)}
										</span>
										{album.releaseEvents
											?.filter((_e, index) => index < 3)
											.map((e, index) => (
												<span key={index}>
													{index === 0 ? <br /> : ', '}
													<Link
														to={EntryUrlMapper.details(
															EntryType.ReleaseEvent,
															e.id,
														)}
													>
														{e.name}
													</Link>
												</span>
											))}
									</td>
									<td style={{ width: '150px' }}>
										<span title={`${album.ratingAverage}`}>
											{albumSearchStore
												.ratingStars?.(album)
												.map((ratingStar, index) => (
													<React.Fragment key={index}>
														{index > 0 && ' '}
														{/* eslint-disable-next-line jsx-a11y/alt-text */}
														<img
															src={
																ratingStar.enabled
																	? '/Content/star.png'
																	: '/Content/star_disabled.png'
															}
														/>
													</React.Fragment>
												))}
										</span>
										<br />
										<span>{album.ratingCount}</span>{' '}
										{t('ViewRes.Search:Index.Times')}
									</td>
								</tr>
							))}
						</tbody>
					</table>
				)}

				{albumSearchStore.viewMode === 'Tiles' && (
					<AlbumSearchListTiles albums={albumSearchStore.page} />
				)}

				<ServerSidePaging pagingStore={albumSearchStore.paging} />
			</>
		);
	},
);

export default AlbumSearchList;
