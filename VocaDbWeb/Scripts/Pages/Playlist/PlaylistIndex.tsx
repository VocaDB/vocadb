import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { DraftIcon } from '@/Components/Shared/Partials/Shared/DraftIcon';
import { SongTypeLabel } from '@/Components/Shared/Partials/Song/SongTypeLabel';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { EntryStatus } from '@/Models/EntryStatus';
import { PVServiceIcons } from '@/Models/PVServiceIcons';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { UrlMapper } from '@/Shared/UrlMapper';
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

		const { vdbPlayer, playerRef } = useVdbPlayer();

		return (
			<Layout
				title={title}
				toolbar={
					<>
						<JQueryUIButton
							as={SafeAnchor}
							onClick={vdbPlayer.clear}
							icons={{ primary: 'ui-icon-trash' }}
							disabled={vdbPlayer.playQueue.isEmpty}
						>
							Clear{/* TODO: localize */}
						</JQueryUIButton>
					</>
				}
			>
				<table
					className="table"
					css={{ backgroundColor: 'rgba(255, 255, 255, 0.9)' }}
				>
					<ReactSortable
						tag="tbody"
						list={vdbPlayer.playQueue.items}
						setList={(items): void =>
							runInAction(() => {
								vdbPlayer.playQueue.items = items;
							})
						}
					>
						{vdbPlayer.playQueue.items.map((item) => (
							<tr
								className={classNames(
									item === vdbPlayer.selectedItem && 'info',
								)}
								key={item.id}
							>
								<td style={{ width: '80px' }}>
									{item.entry.mainPicture && item.entry.mainPicture.urlThumb && (
										<Link
											to={EntryUrlMapper.details_entry(item.entry)}
											title={item.entry.additionalNames}
										>
											{/* eslint-disable-next-line jsx-a11y/alt-text */}
											<img
												src={item.entry.mainPicture.urlThumb}
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
											onClick={(): void => {
												if (vdbPlayer.selectedItem === item) {
													const player = playerRef.current;

													if (!player) return;

													player.seekTo(0);
													player.play();
												} else {
													vdbPlayer.play(item);
												}
											}}
											href="#"
										>
											<i className="icon-play" /> Play{/* TODO: localize */}
										</Button>{' '}
										<Button
											onClick={(): void => vdbPlayer.removeFromQueue(item)}
											href="#"
										>
											<i className="icon-remove" /> {t('ViewRes:Shared.Remove')}
										</Button>
									</div>
									<Link
										to={EntryUrlMapper.details_entry(item.entry)}
										title={item.entry.additionalNames}
									>
										{item.entry.name}
									</Link>{' '}
									{item.entry.songType && (
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
									<DraftIcon
										status={
											EntryStatus[item.entry.status as keyof typeof EntryStatus]
										}
									/>
									<br />
									<small className="extraInfo">{item.entry.artistString}</small>
								</td>
							</tr>
						))}
					</ReactSortable>
				</table>
			</Layout>
		);
	},
);

export default PlaylistIndex;
