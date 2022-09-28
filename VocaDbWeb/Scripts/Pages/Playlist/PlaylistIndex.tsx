import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { EmbedPVPreview } from '@/Components/Shared/Partials/PV/EmbedPVPreview';
import { DraftIcon } from '@/Components/Shared/Partials/Shared/DraftIcon';
import { SongTypeLabel } from '@/Components/Shared/Partials/Song/SongTypeLabel';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { PVServiceIcons } from '@/Models/PVServiceIcons';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { UrlMapper } from '@/Shared/UrlMapper';
import { useNostalgicDiva } from '@vocadb/nostalgic-diva';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { ReactSortable } from 'react-sortablejs';

const urlMapper = new UrlMapper(vdb.values.baseAddress);
const pvServiceIcons = new PVServiceIcons(urlMapper);

const PlaylistIndex = observer(
	(): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes', 'ViewRes.Search']);

		const title = t('ViewRes.Search:Index.Playlist');

		useVocaDbTitle(title, ready);

		const diva = useNostalgicDiva();
		const { playQueue } = useVdbPlayer();

		const handleClickAddToNewSongList = React.useCallback(() => {
			// TODO: Implement.
		}, []);

		return (
			<Layout
				title={title}
				toolbar={
					<>
						{playQueue.currentItem && (
							<div id="pvPlayer" className="song-pv-player">
								<EmbedPVPreview
									entry={playQueue.currentItem.entry}
									pv={playQueue.currentItem.pv}
									allowInline
								/>
							</div>
						)}
						{playQueue.selectedItems.length > 0 ? (
							<>
								<JQueryUIButton
									as={SafeAnchor}
									onClick={playQueue.playSelectedItemsNext}
									icons={{ primary: 'ui-icon-play' }}
								>
									Play next{/* TODO: localize */}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									onClick={playQueue.addSelectedItemsToPlayQueue}
									icons={{ primary: 'ui-icon-plus' }}
								>
									Add to play queue{/* TODO: localize */}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									onClick={handleClickAddToNewSongList}
									icons={{ primary: 'ui-icon-plus' }}
									disabled={true}
									title="Coming soon!" /* TODO: Remove. */
								>
									Add to new song list{/* TODO: localize */}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									onClick={playQueue.removeSelectedItemsFromPlayQueue}
									icons={{ primary: ' ui-icon-close' }}
								>
									Remove{/* TODO: localize */}
								</JQueryUIButton>
							</>
						) : (
							<>
								<JQueryUIButton
									as={SafeAnchor}
									onClick={playQueue.clear}
									icons={{ primary: 'ui-icon-trash' }}
									disabled={playQueue.isEmpty}
								>
									Clear{/* TODO: localize */}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									onClick={playQueue.addSelectedItemsToPlayQueue}
									icons={{ primary: 'ui-icon-plus' }}
									disabled={playQueue.isEmpty}
								>
									Add to play queue{/* TODO: localize */}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									onClick={handleClickAddToNewSongList}
									icons={{ primary: 'ui-icon-plus' }}
									disabled={true}
									title="Coming soon!" /* TODO: Remove. */
								>
									Add to new song list{/* TODO: localize */}
								</JQueryUIButton>
							</>
						)}
					</>
				}
			>
				{!playQueue.isEmpty && (
					<table
						className="table"
						css={{ backgroundColor: 'rgba(255, 255, 255, 0.9)' }}
					>
						<thead>
							<tr>
								<th>
									<input
										type="checkbox"
										checked={playQueue.allItemsSelected}
										onChange={(e): void =>
											runInAction(() => {
												playQueue.allItemsSelected = e.target.checked;
											})
										}
									/>
								</th>
								<th colSpan={2}>
									{
										playQueue.selectedItems.length > 0
											? `${playQueue.selectedItems.length} item(s) selected` /* TODO: localize */
											: 'Name' /* TODO: localize */
									}
								</th>
							</tr>
						</thead>
						<ReactSortable
							tag="tbody"
							list={playQueue.items}
							setList={(items): void =>
								runInAction(() => {
									playQueue.items = items;
								})
							}
						>
							{playQueue.items.map((item) => (
								<tr
									className={classNames(
										item === playQueue.currentItem && 'info',
									)}
									key={item.id}
								>
									<td>
										<input
											type="checkbox"
											checked={item.isSelected}
											onChange={(e): void =>
												runInAction(() => {
													item.isSelected = e.target.checked;
												})
											}
										/>
									</td>
									<td style={{ width: '80px' }}>
										{item.entry.urlThumb && (
											<Link
												to={EntryUrlMapper.details_entry(item.entry)}
												title={item.entry.additionalNames}
											>
												{/* eslint-disable-next-line jsx-a11y/alt-text */}
												<img
													src={item.entry.urlThumb}
													title="Cover picture" /* TODO: localize */
													className="coverPicThumb img-rounded"
													referrerPolicy="same-origin"
												/>
											</Link>
										)}
									</td>
									<td>
										<div className="pull-right">
											<Button
												onClick={async (): Promise<void> => {
													if (playQueue.currentItem === item) {
														await diva.setCurrentTime(0);
													} else {
														playQueue.setCurrentItem(item);
													}
												}}
												href="#"
											>
												<i className="icon-play" /> Play{/* TODO: localize */}
											</Button>{' '}
											<Button
												onClick={(): Promise<void> =>
													playQueue.removeFromPlayQueue([item])
												}
												href="#"
											>
												<i className="icon-remove" />{' '}
												{t('ViewRes:Shared.Remove')}
											</Button>
										</div>
										<Link
											to={EntryUrlMapper.details_entry(item.entry)}
											title={item.entry.additionalNames}
										>
											{item.entry.name}
										</Link>{' '}
										{item.entry.entryType === 'Song' /* TODO: enum */ &&
											item.entry.songType && (
												<>
													{' '}
													<SongTypeLabel songType={item.entry.songType} />
												</>
											)}{' '}
										{pvServiceIcons
											.getIconUrls(item.pv.service)
											.map((icon, index) => (
												<React.Fragment key={icon.service}>
													{index > 0 && ' '}
													{/* eslint-disable-next-line jsx-a11y/alt-text */}
													<img src={icon.url} title={icon.service} />
												</React.Fragment>
											))}{' '}
										<DraftIcon status={item.entry.status} />
										{(item.entry.entryType === 'Album' ||
											item.entry.entryType === 'Song') && (
											<>
												<br />
												<small className="extraInfo">
													{item.entry.artistString}
												</small>
											</>
										)}
									</td>
								</tr>
							))}
						</ReactSortable>
					</table>
				)}

				{playQueue.hasMoreItems && (
					<h3>
						<SafeAnchor onClick={playQueue.loadMore}>
							{t('ViewRes:Shared.ShowMore')}
						</SafeAnchor>
					</h3>
				)}
			</Layout>
		);
	},
);

export default PlaylistIndex;
