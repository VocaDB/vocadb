import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { Layout } from '@/Components/Shared/Layout';
import { LatestCommentsKnockout } from '@/Components/Shared/Partials/Comment/LatestCommentsKnockout';
import { EntryCountBox } from '@/Components/Shared/Partials/EntryCountBox';
import { DeletedBanner } from '@/Components/Shared/Partials/EntryDetails/DeletedBanner';
import { VenueLinkOrVenueName } from '@/Components/Shared/Partials/Event/VenueLinkOrVenueName';
import { ArtistFilters } from '@/Components/Shared/Partials/Knockout/ArtistFilters';
import { Dropdown } from '@/Components/Shared/Partials/Knockout/Dropdown';
import { ServerSidePaging } from '@/Components/Shared/Partials/Knockout/ServerSidePaging';
import { TagFilters } from '@/Components/Shared/Partials/Knockout/TagFilters';
import { PlayList } from '@/Components/Shared/Partials/PlayList';
import { SongAdvancedFilters } from '@/Components/Shared/Partials/Search/AdvancedFilters';
import { DraftIcon } from '@/Components/Shared/Partials/Shared/DraftIcon';
import { DraftMessage } from '@/Components/Shared/Partials/Shared/DraftMessage';
import { EntryStatusMessage } from '@/Components/Shared/Partials/Shared/EntryStatusMessage';
import { PVPreviewKnockout } from '@/Components/Shared/Partials/Song/PVPreviewKnockout';
import { SongTypeLabel } from '@/Components/Shared/Partials/Song/SongTypeLabel';
import { SongTypesDropdownKnockout } from '@/Components/Shared/Partials/Song/SongTypesDropdownKnockout';
import { TagList } from '@/Components/Shared/Partials/TagList';
import { TagsEdit } from '@/Components/Shared/Partials/TagsEdit';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { SongInListContract } from '@/DataContracts/Song/SongInListContract';
import { SongListContract } from '@/DataContracts/Song/SongListContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { ImageSize } from '@/Models/Images/ImageSize';
import { SongType } from '@/Models/Songs/SongType';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { songListRepo } from '@/Repositories/SongListRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { tagRepo } from '@/Repositories/TagRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { urlMapper } from '@/Shared/UrlMapper';
import { ISongSearchItem, SongSortRule } from '@/Stores/Search/SongSearchStore';
import { SongListStore } from '@/Stores/SongList/SongListStore';
import { PlayQueueRepositoryType } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { AutoplayContext } from '@/Stores/VdbPlayer/PlayQueueStore';
import { useVdb } from '@/VdbContext';
import dayjs from '@/dayjs';
import { useLocationStateStore } from '@/route-sphere';
import '@/styles/Styles/songlist.less';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import NProgress from 'nprogress';
import qs from 'qs';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

interface SongListDetailsTableRowProps {
	songListStore: SongListStore;
	item: SongInListContract & {
		song: ISongSearchItem;
	};
}

const SongListDetailsTableRow = observer(
	({
		songListStore,
		item,
	}: SongListDetailsTableRowProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.SongList']);

		return (
			<tr>
				<td style={{ width: '75px' }}>
					{item.song.mainPicture && item.song.mainPicture.urlThumb && (
						<Link
							to={EntryUrlMapper.details_song(item.song)}
							title={item.song.additionalNames}
						>
							{/* eslint-disable-next-line jsx-a11y/alt-text */}
							<img
								src={item.song.mainPicture.urlThumb}
								title="Cover picture" /* LOC */
								className="coverPicThumb img-rounded"
								referrerPolicy="same-origin"
							/>
						</Link>
					)}
				</td>
				<td>
					{item.song.previewStore && item.song.previewStore.pvServices && (
						<div className="pull-right">
							<Button
								onClick={(): void => item.song.previewStore?.togglePreview()}
								className={classNames(
									'previewSong',
									item.song.previewStore.preview && 'active',
								)}
							>
								<i className="icon-film" />{' '}
								{t('ViewRes.SongList:Details.Preview')}
							</Button>
						</div>
					)}
					<span>{item.order}</span>.{' '}
					<Link
						to={EntryUrlMapper.details_song(item.song)}
						title={item.song.additionalNames}
					>
						{item.song.name}
					</Link>
					{item.notes && (
						<>
							{' '}
							<span>
								(<span>{item.notes}</span>)
							</span>
						</>
					)}{' '}
					<SongTypeLabel songType={item.song.songType} />{' '}
					{songListStore.pvServiceIcons
						.getIconUrls(item.song.pvServices)
						.map((icon, index) => (
							<React.Fragment key={icon.service}>
								{index > 0 && ' '}
								{/* eslint-disable-next-line jsx-a11y/alt-text */}
								<img src={icon.url} title={icon.service} />
							</React.Fragment>
						))}{' '}
					<DraftIcon status={item.song.status} />
					<br />
					<small className="extraInfo">{item.song.artistString}</small>
					{item.song.previewStore && item.song.previewStore.pvServices && (
						<PVPreviewKnockout
							previewStore={item.song.previewStore}
							getPvServiceIcons={songListStore.pvServiceIcons.getIconUrls}
						/>
					)}
				</td>
				{songListStore.showTags && (
					<td style={{ width: '33%' }}>
						{item.song.tags && item.song.tags.length > 0 && (
							<div>
								<i className="icon icon-tags fix-icon-margin" />{' '}
								{item.song.tags.map((tag, index) => (
									<React.Fragment key={tag.tag.id}>
										{index > 0 && ', '}
										<Link
											to={songListStore.mapTagUrl(tag)}
											title={tag.tag.additionalNames}
										>
											{tag.tag.name}
										</Link>
									</React.Fragment>
								))}
							</div>
						)}
					</td>
				)}
			</tr>
		);
	},
);

interface SongListDetailsTableBodyProps {
	songListStore: SongListStore;
}

const SongListDetailsTableBody = observer(
	({ songListStore }: SongListDetailsTableBodyProps): React.ReactElement => {
		return (
			<tbody>
				{songListStore.page.map((item, index) => (
					<SongListDetailsTableRow
						songListStore={songListStore}
						item={item}
						key={index}
					/>
				))}
			</tbody>
		);
	},
);

interface SongListDetailsTableProps {
	songListStore: SongListStore;
}

const SongListDetailsTable = observer(
	({ songListStore }: SongListDetailsTableProps): React.ReactElement => {
		return (
			<table className="table table-striped">
				<SongListDetailsTableBody songListStore={songListStore} />
			</table>
		);
	},
);

const usePageProperties = (
	songList: SongListContract,
): {
	pageTitle: string;
	title: string;
	subtitle: string;
	ready: boolean;
} => {
	const { t, ready } = useTranslation(['Resources', 'ViewRes.SongList']);

	if (songList.featuredCategory === 'Nothing') {
		return {
			pageTitle: `${t('ViewRes.SongList:Details.SongList')} - ${songList.name}`,
			title: songList.name,
			subtitle: t('ViewRes.SongList:Details.SongList'),
			ready: ready,
		};
	} else {
		const categoryName = t(
			`Resources:SongListFeaturedCategoryNames.${songList.featuredCategory}`,
		);

		return {
			pageTitle: `${categoryName} - ${songList.name}`,
			title: songList.name,
			subtitle: categoryName,
			ready: ready,
		};
	}
};

interface SongListDetailsLayoutProps {
	songList: SongListContract;
	songListStore: SongListStore;
}

const SongListDetailsLayout = observer(
	({
		songList,
		songListStore,
	}: SongListDetailsLayoutProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.Search',
			'ViewRes.SongList',
			'VocaDb.Web.Resources.Domain',
		]);

		const { pageTitle, title, subtitle, ready } = usePageProperties(songList);

		useLocationStateStore(songListStore);

		const smallThumbUrl = UrlHelper.imageThumb(
			songList.mainPicture,
			ImageSize.SmallThumb,
		);
		const thumbUrl = UrlHelper.imageThumb(
			songList.mainPicture,
			ImageSize.Original,
		);

		const { playQueue } = useVdbPlayer();

		return (
			<Layout
				pageTitle={pageTitle}
				ready={ready}
				title={title}
				subtitle={subtitle}
				parents={
					songList.featuredCategory === 'Nothing' ? (
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details_user_byName(songList.author.name),
							}}
						>
							{songList.author.name}
						</Breadcrumb.Item>
					) : (
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/SongList/Featured?${qs.stringify({
									categoryName: songList.featuredCategory,
								})}`,
							}}
						>
							{t(
								`Resources:SongListFeaturedCategoryNames.${songList.featuredCategory}`,
							)}
						</Breadcrumb.Item>
					)
				}
				// TODO: description
				// TODO: robots
				// TODO: canonicalUrl
				// TODO: head
				toolbar={
					<>
						{loginManager.canEditSongList(songList) && (
							<>
								<JQueryUIButton
									as={Link}
									to={`/SongList/Edit/${songList.id}`}
									icons={{ primary: 'ui-icon-wrench' }}
								>
									{t('ViewRes:Shared.Edit')}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as={Link}
									to={`/SongList/Versions/${songList.id}`}
									icons={{ primary: 'ui-icon-clock' }}
								>
									{t('ViewRes:EntryDetails.ViewModifications')}
								</JQueryUIButton>
							</>
						)}{' '}
						<JQueryUIButton
							as="a"
							href={`/SongList/Export/${songList.id}`}
							icons={{ primary: 'ui-icon-arrowthickstop-1-s' }}
						>
							{t('ViewRes.SongList:Details.Export')}
						</JQueryUIButton>
						{songList.featuredCategory !== 'Nothing' && (
							<>
								{' '}
								<EntryStatusMessage status={songList.status} />
							</>
						)}
					</>
				}
			>
				{songList.featuredCategory !== 'Nothing' &&
					songList.status === EntryStatus.Draft &&
					!songList.deleted && <DraftMessage section="" />}

				{songList.deleted && <DeletedBanner />}

				<div className="media">
					{smallThumbUrl && (
						<a className="pull-left" href={thumbUrl}>
							<img
								className="media-object"
								src={smallThumbUrl}
								alt="Thumb" /* LOC */
							/>
						</a>
					)}
					<div className="media-body">
						{songList.eventDate && (
							<p>
								{t('ViewRes.SongList:Details.Date', {
									0: dayjs.utc(songList.eventDate).format('l'),
								})}
							</p>
						)}
						{songList.description && (
							<Markdown>{songList.description}</Markdown>
						)}

						<div style={{ margin: '0 0 10px' }}>
							<div className="inline-block">{t('ViewRes:Shared.Tags')}:</div>
							{songListStore.tagUsages.tagUsages.length > 0 && (
								<>
									{' '}
									<div className="entry-tag-usages inline-block">
										<TagList tagListStore={songListStore.tagUsages} />
									</div>
								</>
							)}
							<div>
								<JQueryUIButton
									as={SafeAnchor}
									className="inline-block"
									disabled={
										!loginManager.canEditTagsForEntry({
											...songList,
											entryType: EntryType.SongList,
										})
									}
									icons={{ primary: 'ui-icon-tag' }}
									onClick={songListStore.tagsEditStore.show}
									href="#"
								>
									{t('ViewRes:EntryDetails.EditTags')}
								</JQueryUIButton>
							</div>
						</div>

						{songList.events && songList.events.length > 0 && (
							<div style={{ margin: '0 0 10px' }}>
								<h4>{t('ViewRes.SongList:Details.Events')}</h4>
								<ul>
									{songList.events.map((event) => (
										<li key={event.id}>
											<Link
												to={EntryUrlMapper.details(
													EntryType.ReleaseEvent,
													event.id,
													event.urlSlug,
												)}
											>
												{event.name}
											</Link>{' '}
											<small>
												(
												{event.date ? (
													event.venue || event.venueName ? (
														<>
															{dayjs(event.date).format('l')}
															, <VenueLinkOrVenueName event={event} />
														</>
													) : (
														dayjs(event.date).format('l')
													)
												) : (
													(event.venue || event.venueName) && (
														<VenueLinkOrVenueName event={event} />
													)
												)}
												)
											</small>
										</li>
									))}
								</ul>
							</div>
						)}
					</div>
				</div>

				<div className="clearfix well well-transparent">
					<div className="songlist-mode-selection pull-left">
						<ButtonGroup>
							<Button
								onClick={async (): Promise<void> => {
									await playQueue.startAutoplay(
										new AutoplayContext(
											PlayQueueRepositoryType.SongList,
											songListStore.queryParams,
											false,
										),
									);
								}}
								title="Play" /* LOC */
								className="btn-nomargin"
							>
								<i className="icon-play noMargin" /> Play{/* LOC */}
							</Button>
						</ButtonGroup>
						<ButtonGroup>
							<Button
								onClick={async (): Promise<void> => {
									await playQueue.startAutoplay(
										new AutoplayContext(
											PlayQueueRepositoryType.SongList,
											songListStore.queryParams,
											true,
										),
									);
								}}
								title="Shuffle and play" /* LOC */
								className="btn-nomargin"
							>
								<i className="icon icon-random" /> Shuffle and play{/* LOC */}
							</Button>
						</ButtonGroup>
					</div>
					{!songListStore.playlistMode && (
						<Button
							className={classNames(
								songListStore.showTags && 'active',
								'pull-left',
							)}
							onClick={(): void =>
								runInAction(() => {
									songListStore.showTags = !songListStore.showTags;
								})
							}
							href="#"
							title={t('ViewRes.SongList:Details.ShowTags')}
						>
							<i className="icon-tags" />
						</Button>
					)}
					<div className="inline-block songlist-sort pull-left">
						{t('ViewRes:EntryIndex.SortBy')}{' '}
						<Dropdown
							items={{
								'': t('ViewRes.SongList:Details.DefaultSortRule'),
								...Object.fromEntries(
									Object.values(SongSortRule).map((value) => [
										value,
										t(`Resources:SongSortRuleNames.${value}`),
									]),
								),
							}}
							value={songListStore.sort}
							onChange={(value): void =>
								runInAction(() => {
									songListStore.sort = value;
								})
							}
						/>
					</div>
					<div className="pull-left songlist-text-query">
						<i className="icon-search" />{' '}
						<DebounceInput
							type="text"
							value={songListStore.query}
							onChange={(e): void =>
								runInAction(() => {
									songListStore.query = e.target.value;
								})
							}
							placeholder={t('ViewRes:Shared.Search')}
							maxLength={200}
							debounceTimeout={300}
						/>
					</div>
					&nbsp;
					<Button
						onClick={(): void =>
							runInAction(() => {
								songListStore.showAdvancedFilters =
									!songListStore.showAdvancedFilters;
							})
						}
						className={classNames(
							songListStore.showAdvancedFilters && 'active',
						)}
					>
						{t('ViewRes.Search:Index.MoreFilters')} <span className="caret" />
					</Button>
					{songListStore.showAdvancedFilters && (
						<div className="form-horizontal withMargin">
							<div className="control-group">
								<div className="control-label">
									{t('ViewRes.Search:Index.SongType')}
								</div>
								<div className="controls">
									<SongTypesDropdownKnockout
										activeKey={songListStore.songType}
										onSelect={(eventKey): void =>
											runInAction(() => {
												songListStore.songType = eventKey as SongType;
											})
										}
									/>
								</div>
							</div>

							<div className="control-group">
								<div className="control-label">
									{t('VocaDb.Web.Resources.Domain:EntryTypeNames.Artist')}
								</div>
								<div className="controls">
									<ArtistFilters
										artistFilters={songListStore.artistFilters}
										artistParticipationStatus={false}
									/>
								</div>
							</div>

							<div className="control-group">
								<div className="control-label">{t('ViewRes:Shared.Tags')}</div>
								<div className="controls">
									<TagFilters tagFilters={songListStore.tagFilters} />
								</div>
							</div>

							<div className="control-group">
								<div className="control-label"></div>
								<div className="controls">
									<SongAdvancedFilters
										advancedFilters={songListStore.advancedFilters}
									/>
								</div>
							</div>
						</div>
					)}
				</div>

				{songListStore.playlistMode ? (
					<div className="well well-transparent songlist-playlist">
						<PlayList />
					</div>
				) : (
					<div className={classNames(songListStore.loading && 'loading')}>
						<EntryCountBox pagingStore={songListStore.paging} />

						<ServerSidePaging pagingStore={songListStore.paging} />

						<SongListDetailsTable songListStore={songListStore} />

						<ServerSidePaging pagingStore={songListStore.paging} />
					</div>
				)}

				<LatestCommentsKnockout
					editableCommentsStore={songListStore.comments}
				/>

				<TagsEdit tagsEditStore={songListStore.tagsEditStore} />
			</Layout>
		);
	},
);

const SongListDetails = (): React.ReactElement => {
	const vdb = useVdb();
	const loginManager = useLoginManager();

	const [model, setModel] = React.useState<
		{ songList: SongListContract; songListStore: SongListStore } | undefined
	>();

	const { id } = useParams();

	React.useEffect(() => {
		NProgress.start();

		songListRepo
			.getDetails({ id: Number(id) })
			.then((songList) => {
				setModel({
					songList: songList,
					songListStore: new SongListStore(
						vdb.values,
						loginManager,
						urlMapper,
						songListRepo,
						songRepo,
						tagRepo,
						userRepo,
						artistRepo,
						songList.latestComments ?? [],
						songList.id,
						songList.commentsLocked ?? false,
						songList.tags ?? [],
						loginManager.canDeleteComments,
						songList.featuredCategory,
					),
				});

				NProgress.done();
			})
			.catch((error) => {
				if (error.response) {
					if (error.response.status === 404)
						window.location.href = '/Error/NotFound';
				}

				throw error;
			});
	}, [vdb, loginManager, id]);

	return model ? (
		<SongListDetailsLayout
			songList={model.songList}
			songListStore={model.songListStore}
		/>
	) : (
		<></>
	);
};

export default SongListDetails;
