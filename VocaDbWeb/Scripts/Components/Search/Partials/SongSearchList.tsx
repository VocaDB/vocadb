import Button from '@Bootstrap/Button';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import EntryCountBox from '@Components/Shared/Partials/EntryCountBox';
import ServerSidePaging from '@Components/Shared/Partials/Knockout/ServerSidePaging';
import DraftIcon from '@Components/Shared/Partials/Shared/DraftIcon';
import SongTypeLabel from '@Components/Shared/Partials/Song/SongTypeLabel';
import EntryStatus from '@Models/EntryStatus';
import SongVoteRating from '@Models/SongVoteRating';
import SongType from '@Models/Songs/SongType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import SongSearchStore, { SongSortRule } from '@Stores/Search/SongSearchStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SongSearchListProps {
	songSearchStore: SongSearchStore;
}

const SongSearchList = observer(
	({ songSearchStore }: SongSearchListProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes', 'ViewRes.Search']);

		return (
			<>
				{songSearchStore.viewMode === 'Details' && (
					<>
						<EntryCountBox
							pagingStore={songSearchStore.paging}
							onPageSizeChange={(pageSize): void =>
								runInAction(() => {
									// TODO: use redial
									songSearchStore.paging.pageSize = pageSize;
								})
							}
						/>

						<ServerSidePaging
							pagingStore={songSearchStore.paging}
							onPageChange={(page): void =>
								runInAction(() => {
									// TODO: use redial
									songSearchStore.paging.page = page;
								})
							}
						/>

						<table
							className={classNames(
								'table',
								'table-striped',
								songSearchStore.loading && 'loading',
							)}
						>
							<thead>
								<tr>
									<th colSpan={2}>
										<SafeAnchor
											onClick={(): void =>
												runInAction(() => {
													songSearchStore.sort = SongSortRule.Name;
												})
											}
										>
											{t('ViewRes:Shared.Name')}
											{songSearchStore.sort === SongSortRule.Name && (
												<>
													{' '}
													<span className="sortDirection_down" />
												</>
											)}
										</SafeAnchor>
									</th>
									{songSearchStore.showTags && (
										<th>{t('ViewRes:Shared.Tags')}</th>
									)}
									<th>
										<SafeAnchor
											onClick={(): void =>
												runInAction(() => {
													songSearchStore.sort = SongSortRule.RatingScore;
												})
											}
										>
											{t('ViewRes.Search:Index.Rating')}
											{songSearchStore.sort === SongSortRule.RatingScore && (
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
								{songSearchStore.page.map((song) => (
									<tr key={song.id}>
										<td style={{ width: '80px' }}>
											{song.thumbUrl && (
												<a
													href={EntryUrlMapper.details_song(song)}
													title={song.additionalNames}
												>
													{/* eslint-disable-next-line jsx-a11y/alt-text */}
													<img
														src={song.thumbUrl}
														title="Cover picture" /* TODO: localize */
														className="coverPicThumb img-rounded"
														referrerPolicy="same-origin"
													/>
												</a>
											)}
										</td>
										<td>
											{song.previewStore && song.previewStore.pvServices && (
												<div className="pull-right">
													<Button
														/* TODO: togglePreview */ className="previewSong"
														href="#"
													>
														<i className="icon-film" />{' '}
														{t('ViewRes.Search:Index.Preview')}
													</Button>
												</div>
											)}
											<a
												href={EntryUrlMapper.details_song(song)}
												title={song.additionalNames}
											>
												{song.name}
											</a>{' '}
											<SongTypeLabel
												songType={
													SongType[song.songType as keyof typeof SongType]
												}
											/>{' '}
											{songSearchStore
												.getPVServiceIcons(song.pvServices)
												.map((icon, index) => (
													<React.Fragment key={icon.service}>
														{index > 0 && ' '}
														{/* eslint-disable-next-line jsx-a11y/alt-text */}
														<img src={icon.url} title={icon.service} />
													</React.Fragment>
												))}
											{false /* TODO */ && (
												<>
													{' '}
													<span
														className="icon heartIcon"
														title={t(
															`Resources:SongVoteRatingNames.${
																SongVoteRating[SongVoteRating.Favorite]
															}`,
														)}
													/>
												</>
											)}
											{false /* TODO */ && (
												<>
													{' '}
													<span
														className="icon starIcon"
														title={t(
															`Resources:SongVoteRatingNames.${
																SongVoteRating[SongVoteRating.Like]
															}`,
														)}
													/>
												</>
											)}{' '}
											<DraftIcon
												status={
													EntryStatus[song.status as keyof typeof EntryStatus]
												}
											/>
											{/* TODO: icon-calendar */}
											<br />
											<small className="extraInfo">{song.artistString}</small>
											{/* TODO: PVPreviewKnockout */}
										</td>
										{songSearchStore.showTags && (
											<td className="search-tags-column">
												{song.tags && song.tags.length > 0 && (
													<div>
														<i className="icon icon-tags fix-icon-margin" />{' '}
														{song.tags.map((tag, index) => (
															<React.Fragment key={tag.tag.id}>
																{index > 0 && ', '}
																<SafeAnchor
																	onClick={(): void =>
																		songSearchStore.selectTag(tag.tag)
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
											<span>{song.ratingScore}</span>{' '}
											{t('ViewRes.Search:Index.TotalScore')}
										</td>
									</tr>
								))}
							</tbody>
						</table>

						<ServerSidePaging
							pagingStore={songSearchStore.paging}
							onPageChange={(page): void =>
								runInAction(() => {
									// TODO: use redial
									songSearchStore.paging.page = page;
								})
							}
						/>
					</>
				)}

				{songSearchStore.viewMode === 'PlayList' && (
					<>{/* TODO: _PlayList */}</>
				)}
			</>
		);
	},
);

export default SongSearchList;
