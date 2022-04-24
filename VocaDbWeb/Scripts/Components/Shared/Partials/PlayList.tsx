import Button from '@Bootstrap/Button';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import PVPlayerStore from '@Stores/PVs/PVPlayerStore';
import PlayListStore from '@Stores/Song/PlayList/PlayListStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

import EmbedPV from './PV/EmbedPV';
import PVRatingButtonsForIndex from './PVRatingButtonsForIndex';
import SongTypeLabel from './Song/SongTypeLabel';

interface PlayListProps {
	playListStore: PlayListStore;
	pvPlayerStore: PVPlayerStore;
}

const PlayList = observer(
	({ playListStore, pvPlayerStore }: PlayListProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes.Home',
			'VocaDb.Web.Resources.Views.Shared.Partials',
		]);

		const tableElement = React.useRef<HTMLTableElement>(undefined!);

		return (
			<>
				<div className="songlist-playlist-player">
					<div>
						{pvPlayerStore.selectedSong && pvPlayerStore.selectedSong.song && (
							<div>
								<h4 className="pull-left">
									<Link
										to={EntryUrlMapper.details_song(
											pvPlayerStore.selectedSong.song,
										)}
									>
										{pvPlayerStore.selectedSong.song.name}
									</Link>{' '}
									<span className="songlist-playlist-player-artists">
										{pvPlayerStore.selectedSong.song.artistString}
									</span>
								</h4>

								{pvPlayerStore.ratingButtonsStore && (
									<PVRatingButtonsForIndex
										pvRatingButtonsStore={pvPlayerStore.ratingButtonsStore}
									/>
								)}

								{pvPlayerStore.primaryPV && (
									<div id="pv-player-wrapper">
										<EmbedPV
											pv={pvPlayerStore.primaryPV}
											enableApi={true}
											id="pv-player"
										/>
									</div>
								)}
							</div>
						)}
						<Button
							href="#"
							onClick={playListStore.nextSong}
							className={classNames(
								playListStore.paging.totalItems === 0 && 'disabled',
							)}
							title={t(
								'VocaDb.Web.Resources.Views.Shared.Partials:PlayList.NextSong',
							)}
						>
							<i className="icon-step-forward noMargin" />
						</Button>{' '}
						<Button
							href="#"
							onClick={(): void =>
								runInAction(() => {
									pvPlayerStore.shuffle = !pvPlayerStore.shuffle;
								})
							}
							className={classNames(pvPlayerStore.shuffle && 'active')}
							title={t(
								'VocaDb.Web.Resources.Views.Shared.Partials:PlayList.Shuffle',
							)}
						>
							<i className="icon icon-random noMargin" />
						</Button>{' '}
						<Button
							href="#"
							onClick={(): void =>
								runInAction(() => {
									pvPlayerStore.autoplay = !pvPlayerStore.autoplay;
								})
							}
							className={classNames(pvPlayerStore.autoplay && 'active')}
							title={t(
								'VocaDb.Web.Resources.Views.Shared.Partials:PlayList.AutoplayNote',
							)}
						>
							<i className="icon icon-play noMargin" />{' '}
							{t(
								'VocaDb.Web.Resources.Views.Shared.Partials:PlayList.Autoplay',
							)}
						</Button>
						{pvPlayerStore.selectedSong && pvPlayerStore.selectedSong.song && (
							<Button
								as={Link}
								variant="info"
								className="song-info pull-right"
								to={EntryUrlMapper.details(
									EntryType.Song,
									pvPlayerStore.selectedSong?.song.id,
								)}
							>
								<i className="icon icon-info-sign" />{' '}
								{t('ViewRes.Home:Index.ViewSongInfo')}
							</Button>
						)}
					</div>
				</div>

				<div className="songlist-playlist-songs-wrapper">
					<table
						className="table table-condensed songlist-playlist-songs"
						ref={tableElement}
						onScroll={(): void => {
							if (
								tableElement.current.scrollHeight -
									tableElement.current.scrollTop ===
								tableElement.current.clientHeight
							) {
								playListStore.scrollEnd();
							}
						}}
					>
						<tbody>
							{playListStore.page.map((song) => (
								<tr
									className={classNames(
										pvPlayerStore.selectedSong &&
											pvPlayerStore.selectedSong.song.id === song.song.id &&
											'active',
									)}
									onClick={(): void =>
										runInAction(() => {
											pvPlayerStore.selectedSong = song;
										})
									}
									key={song.song.id}
								>
									<td style={{ width: '30px' }}>
										{song.song.mainPicture && song.song.mainPicture.urlThumb && (
											<a
												href={EntryUrlMapper.details(
													EntryType.Song,
													song.song.id,
												)}
												title={song.song.additionalNames}
												onClick={(e): void => e.preventDefault()}
											>
												{/* eslint-disable-next-line jsx-a11y/alt-text */}
												<img
													src={song.song.mainPicture.urlThumb}
													title="Cover picture" /* TODO: localize */
													className="coverPicIcon img-rounded"
													referrerPolicy="same-origin"
												/>
											</a>
										)}
									</td>
									<td>
										<a
											href={EntryUrlMapper.details(
												EntryType.Song,
												song.song.id,
											)}
											title={song.song.additionalNames}
											onClick={(e): void => e.preventDefault()}
										>
											{song.name}
										</a>
									</td>
									<td>
										<SongTypeLabel songType={song.song.songType} />
									</td>
									<td>
										{song.song.lengthSeconds && (
											<span>
												{playListStore.formatLength(song.song.lengthSeconds)}
											</span>
										)}
									</td>
								</tr>
							))}
						</tbody>
					</table>
				</div>
			</>
		);
	},
);

export default PlayList;
