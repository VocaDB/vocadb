import SafeAnchor from '@Bootstrap/SafeAnchor';
import { AlbumToolTip } from '@Components/KnockoutExtensions/EntryToolTip';
import EntryCountBox from '@Components/Shared/Partials/EntryCountBox';
import ServerSidePaging from '@Components/Shared/Partials/Knockout/ServerSidePaging';
import DraftIcon from '@Components/Shared/Partials/Shared/DraftIcon';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import AlbumSearchStore, {
	AlbumSortRule,
} from '@Stores/Search/AlbumSearchStore';
import classNames from 'classnames';
import _ from 'lodash';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface AlbumSearchListProps {
	albumSearchStore: AlbumSearchStore;
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
										<a
											href={EntryUrlMapper.details(EntryType.Album, album.id)}
											title={album.additionalNames}
											className="coverPicThumb"
										>
											{/* eslint-disable-next-line jsx-a11y/img-redundant-alt */}
											<img
												src={
													album.mainPicture?.urlTinyThumb ??
													'/Content/unknown.png'
												}
												title="Cover picture" /* TODO: localize */
												alt="Cover picture" /* TODO: localize */
												className="coverPicThumb img-rounded"
											/>
										</a>
									</td>
									<td>
										<a
											href={EntryUrlMapper.details(EntryType.Album, album.id)}
											title={album.additionalNames}
										>
											{_.truncate(album.name, { length: 150 })}
										</a>{' '}
										<DraftIcon
											status={
												EntryStatus[album.status as keyof typeof EntryStatus]
											}
										/>
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
													<i className="icon icon-tags" />{' '}
													{album.tags.map((tag, index) => (
														<React.Fragment key={tag.tag.id}>
															{index > 0 && ', '}
															<SafeAnchor
																onClick={(): void =>
																	albumSearchStore.selectTag(tag.tag)
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
										<span>{album.releaseDate?.formatted}</span>
										{album.releaseEvent && (
											<span>
												<br />
												<Link
													to={EntryUrlMapper.details(
														EntryType.ReleaseEvent,
														album.releaseEvent.id,
													)}
												>
													{album.releaseEvent.name}
												</Link>
											</span>
										)}
									</td>
									<td style={{ width: '150px' }}>
										<span title={`${album.ratingAverage}`}>
											{albumSearchStore
												.ratingStars(album)
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
					<ul className="smallThumbs">
						{albumSearchStore.page.map((album) => (
							<li key={album.id}>
								<a
									href={EntryUrlMapper.details(EntryType.Album, album.id)}
									title={album.additionalNames}
								>
									<div className="pictureFrame img-rounded">
										<AlbumToolTip
											as="img"
											id={album.id}
											src={
												album.mainPicture?.urlSmallThumb ??
												'/Content/unknown.png'
											}
											alt="Preview" /* TODO: localize */
											className="coverPic img-rounded"
										/>
									</div>
								</a>
								<p>{album.name}</p>
							</li>
						))}
					</ul>
				)}

				<ServerSidePaging pagingStore={albumSearchStore.paging} />
			</>
		);
	},
);

export default AlbumSearchList;
