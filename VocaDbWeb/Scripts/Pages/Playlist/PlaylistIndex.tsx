import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import Dropdown from '@/Bootstrap/Dropdown';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { ArtistFilters } from '@/Components/Shared/Partials/Knockout/ArtistFilters';
import { TagFilters } from '@/Components/Shared/Partials/Knockout/TagFilters';
import { EmbedPVPreview } from '@/Components/Shared/Partials/PV/EmbedPVPreview';
import { DraftIcon } from '@/Components/Shared/Partials/Shared/DraftIcon';
import { SongTypeLabel } from '@/Components/Shared/Partials/Song/SongTypeLabel';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { VdbPlayerEntryLink } from '@/Components/VdbPlayer/VdbPlayerEntryLink';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import JQueryUIDialog from '@/JQueryUI/JQueryUIDialog';
import { PVServiceIcons } from '@/Models/PVServiceIcons';
import { urlMapper } from '@/Shared/UrlMapper';
import { PlayMethod, PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';
import { MoreHorizontal20Filled } from '@fluentui/react-icons';
import { useNostalgicDiva } from '@vocadb/nostalgic-diva';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { ReactSortable } from 'react-sortablejs';

interface SkipListEditProps {
	open: boolean;
	onClose: () => void;
}

const SkipListEdit = observer(
	({ open, onClose }: SkipListEditProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'VocaDb.Web.Resources.Domain']);

		const { playQueue } = useVdbPlayer();

		return (
			<JQueryUIDialog
				title="Edit skip list" /* LOC */
				autoOpen={open}
				width={550}
				close={onClose}
				buttons={[
					{
						text: 'Done' /* LOC */,
						click: onClose,
					},
				]}
			>
				<div className="form-horizontal">
					<div className="control-group">
						<div className="controls">
							<label className="checkbox">
								<input
									type="checkbox"
									checked={playQueue.skipList.removeFromPlayQueueOnSkip}
									onChange={(e): void =>
										runInAction(() => {
											playQueue.skipList.removeFromPlayQueueOnSkip =
												e.target.checked;
										})
									}
								/>
								Remove from play queue on skip{/* LOC */}
							</label>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							{t('VocaDb.Web.Resources.Domain:EntryTypeNames.Artist')}
						</div>
						<div className="controls">
							<ArtistFilters
								artistFilters={playQueue.skipList.artistFilters}
								artistParticipationStatus={false}
								showChildVoicebanks={false}
							/>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">{t('ViewRes:Shared.Tag')}</div>
						<div className="controls">
							<TagFilters
								tagFilters={playQueue.skipList.tagFilters}
								showChildTags={false}
							/>
						</div>
					</div>
				</div>
			</JQueryUIDialog>
		);
	},
);

const PlaylistTableHeader = observer(
	(): React.ReactElement => {
		const { playQueue } = useVdbPlayer();

		return (
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
								? `${playQueue.selectedItems.length} item(s) selected` /* LOC */
								: 'Name' /* LOC */
						}
					</th>
				</tr>
			</thead>
		);
	},
);

interface PlaylistTableRowDropdownProps {
	item: PlayQueueItem;
}

const PlaylistTableRowDropdown = observer(
	({ item }: PlaylistTableRowDropdownProps): React.ReactElement => {
		const { playQueue } = useVdbPlayer();

		const play = React.useCallback(
			(method: PlayMethod) => playQueue.play(method, [item.clone()]),
			[item, playQueue],
		);

		return (
			<Dropdown as={ButtonGroup}>
				<Dropdown.Toggle>
					<span
						css={{
							display: 'flex',
							justifyContent: 'center',
							alignItems: 'center',
						}}
					>
						<MoreHorizontal20Filled />
					</span>
				</Dropdown.Toggle>
				<Dropdown.Menu align="end">
					<Dropdown.Item onClick={(): void => play(PlayMethod.PlayFirst)}>
						Play first{/* LOC */}
					</Dropdown.Item>
					<Dropdown.Item onClick={(): void => play(PlayMethod.PlayNext)}>
						Play next{/* LOC */}
					</Dropdown.Item>
					<Dropdown.Item onClick={(): void => play(PlayMethod.AddToPlayQueue)}>
						Add to play queue{/* LOC */}
					</Dropdown.Item>
					<Dropdown.Divider />
					<Dropdown.Item
						onClick={(): Promise<void> => playQueue.removeItemsAbove(item)}
					>
						Remove to the top{/* LOC */}
					</Dropdown.Item>
					<Dropdown.Item
						onClick={(): Promise<void> => playQueue.removeOtherItems(item)}
					>
						Remove others{/* LOC */}
					</Dropdown.Item>
				</Dropdown.Menu>
			</Dropdown>
		);
	},
);

const pvServiceIcons = new PVServiceIcons(urlMapper);

interface PlaylistTableRowProps {
	item: PlayQueueItem;
}

const PlaylistTableRow = observer(
	({ item }: PlaylistTableRowProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const diva = useNostalgicDiva();
		const { playQueue } = useVdbPlayer();

		return (
			<tr className={classNames(item === playQueue.currentItem && 'info')}>
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
						<VdbPlayerEntryLink
							entry={item.entry}
							title={item.entry.additionalNames}
						>
							{/* eslint-disable-next-line jsx-a11y/alt-text */}
							<img
								src={item.entry.urlThumb}
								title="Cover picture" /* LOC */
								className="coverPicThumb img-rounded"
								referrerPolicy="same-origin"
							/>
						</VdbPlayerEntryLink>
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
						>
							<i className="icon-play" /> Play{/* LOC */}
						</Button>{' '}
						<Button
							onClick={(): Promise<void> =>
								playQueue.removeFromPlayQueue([item])
							}
						>
							<i className="icon-remove" /> {t('ViewRes:Shared.Remove')}
						</Button>{' '}
						<PlaylistTableRowDropdown item={item} />
					</div>
					<VdbPlayerEntryLink
						entry={item.entry}
						title={item.entry.additionalNames}
					>
						{item.entry.name}
					</VdbPlayerEntryLink>{' '}
					{item.entry.entryType === 'Song' /* TODO: enum */ &&
						item.entry.songType && (
							<>
								{' '}
								<SongTypeLabel songType={item.entry.songType} />
							</>
						)}{' '}
					{pvServiceIcons.getIconUrls(item.pv.service).map((icon, index) => (
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
							<small className="extraInfo">{item.entry.artistString}</small>
						</>
					)}
				</td>
			</tr>
		);
	},
);

const PlaylistTableBody = observer(
	(): React.ReactElement => {
		const { playQueue } = useVdbPlayer();

		return (
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
					<PlaylistTableRow item={item} key={item.id} />
				))}
			</ReactSortable>
		);
	},
);

const PlaylistTable = observer(
	(): React.ReactElement => {
		return (
			<table
				className="table"
				css={{ backgroundColor: 'rgba(255, 255, 255, 0.9)' }}
			>
				<PlaylistTableHeader />
				<PlaylistTableBody />
			</table>
		);
	},
);

const PlaylistIndex = observer(
	(): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes', 'ViewRes.Search']);

		const title = t('ViewRes.Search:Index.Playlist');

		const { playQueue } = useVdbPlayer();

		const handleClickAddToNewSongList = React.useCallback(() => {
			// TODO: Implement.
		}, []);

		const [skipListDialogOpen, setSkipListDialogOpen] = React.useState(false);

		return (
			<Layout
				pageTitle={title}
				ready={ready}
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
						<div css={{ display: 'flex' }}>
							<div>
								<JQueryUIButton
									as={SafeAnchor}
									onClick={playQueue.playSelectedItemsNext}
									icons={{ primary: 'ui-icon-play' }}
									disabled={playQueue.selectedItems.length === 0}
								>
									Play next{/* LOC */}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									onClick={playQueue.addSelectedItemsToPlayQueue}
									icons={{ primary: 'ui-icon-plus' }}
									disabled={playQueue.selectedItems.length === 0}
								>
									Add to play queue{/* LOC */}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									onClick={handleClickAddToNewSongList}
									icons={{ primary: 'ui-icon-plus' }}
									disabled={true}
									title="Coming soon!" /* TODO: Remove. */
								>
									Add to new song list{/* LOC */}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									onClick={playQueue.removeSelectedItemsFromPlayQueue}
									icons={{ primary: ' ui-icon-close' }}
									disabled={playQueue.selectedItems.length === 0}
								>
									Remove{/* LOC */}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									onClick={playQueue.clear}
									icons={{ primary: 'ui-icon-trash' }}
									disabled={playQueue.isEmpty}
								>
									Clear{/* LOC */}
								</JQueryUIButton>
							</div>
							<div css={{ flexGrow: 1 }} />
							<div>
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									onClick={(): void => setSkipListDialogOpen(true)}
								>
									Edit skip list{/* LOC */}
								</JQueryUIButton>
							</div>
						</div>
					</>
				}
			>
				{!playQueue.isEmpty && <PlaylistTable />}

				{playQueue.hasMoreItems && (
					<h3>
						<SafeAnchor onClick={playQueue.loadMore}>
							{t('ViewRes:Shared.ShowMore')}
						</SafeAnchor>
					</h3>
				)}

				<SkipListEdit
					open={skipListDialogOpen}
					onClose={(): void => setSkipListDialogOpen(false)}
				/>
			</Layout>
		);
	},
);

export default PlaylistIndex;
