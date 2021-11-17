import Breadcrumb from '@Bootstrap/Breadcrumb';
import Button from '@Bootstrap/Button';
import ButtonGroup from '@Bootstrap/ButtonGroup';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import Markdown from '@Components/KnockoutExtensions/Markdown';
import Layout from '@Components/Shared/Layout';
import LatestCommentsKnockout from '@Components/Shared/Partials/Comment/LatestCommentsKnockout';
import EntryCountBox from '@Components/Shared/Partials/EntryCountBox';
import DeletedBanner from '@Components/Shared/Partials/EntryDetails/DeletedBanner';
import VenueLinkOrVenueName from '@Components/Shared/Partials/Event/VenueLinkOrVenueName';
import ArtistFilters from '@Components/Shared/Partials/Knockout/ArtistFilters';
import Dropdown from '@Components/Shared/Partials/Knockout/Dropdown';
import ServerSidePaging from '@Components/Shared/Partials/Knockout/ServerSidePaging';
import TagFilters from '@Components/Shared/Partials/Knockout/TagFilters';
import PlayList from '@Components/Shared/Partials/PlayList';
import { SongAdvancedFilters } from '@Components/Shared/Partials/Search/AdvancedFilters';
import DraftIcon from '@Components/Shared/Partials/Shared/DraftIcon';
import DraftMessage from '@Components/Shared/Partials/Shared/DraftMessage';
import EntryStatusMessage from '@Components/Shared/Partials/Shared/EntryStatusMessage';
import PVPreviewKnockout from '@Components/Shared/Partials/Song/PVPreviewKnockout';
import SongTypeLabel from '@Components/Shared/Partials/Song/SongTypeLabel';
import SongTypesDropdownKnockout from '@Components/Shared/Partials/Song/SongTypesDropdownKnockout';
import TagList from '@Components/Shared/Partials/TagList';
import TagsEdit from '@Components/Shared/Partials/TagsEdit';
import useRouteParamsTracking from '@Components/useRouteParamsTracking';
import useScript from '@Components/useScript';
import useStoreWithPaging from '@Components/useStoreWithPaging';
import useVocaDbTitle from '@Components/useVocaDbTitle';
import SongListContract from '@DataContracts/Song/SongListContract';
import UrlHelper from '@Helpers/UrlHelper';
import JQueryUIButton from '@JQueryUI/JQueryUIButton';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import ImageSize from '@Models/Images/ImageSize';
import LoginManager from '@Models/LoginManager';
import SongType from '@Models/Songs/SongType';
import ArtistRepository from '@Repositories/ArtistRepository';
import SongListRepository from '@Repositories/SongListRepository';
import SongRepository from '@Repositories/SongRepository';
import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import PVPlayersFactory from '@Stores/PVs/PVPlayersFactory';
import { SongSortRule } from '@Stores/Search/SongSearchStore';
import SongListStore from '@Stores/SongList/SongListStore';
import classNames from 'classnames';
import _ from 'lodash';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import moment from 'moment';
import qs from 'qs';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

import '../../../wwwroot/Content/Styles/songlist.css';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const songListRepo = new SongListRepository(httpClient, urlMapper);
const songRepo = new SongRepository(httpClient, vdb.values.baseAddress);
const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);

const pvPlayersFactory = new PVPlayersFactory();

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
		const { t } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.Search',
			'ViewRes.SongList',
			'VocaDb.Web.Resources.Domain',
		]);

		const { pageTitle, title, subtitle, ready } = usePageProperties(songList);

		useVocaDbTitle(pageTitle, ready);

		useStoreWithPaging(songListStore);
		useRouteParamsTracking(songListStore, ready);

		useScript('/Scripts/soundcloud-api.js');
		useScript('https://www.youtube.com/iframe_api');

		const smallThumbUrl = UrlHelper.imageThumb(
			songList.mainPicture,
			ImageSize.SmallThumb,
		);
		const thumbUrl = UrlHelper.imageThumb(
			songList.mainPicture,
			ImageSize.Original,
		);

		return (
			<Layout
				title={title}
				subtitle={subtitle}
				parents={
					songList.featuredCategory === 'Nothing' ? (
						<Breadcrumb.Item
							href={EntryUrlMapper.details_user_byName(songList.author.name)}
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
									as="a"
									href={`/SongList/Edit/${songList.id}`}
									icons={{ primary: 'ui-icon-wrench' }}
								>
									{t('ViewRes:Shared.Edit')}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as="a"
									href={`/SongList/Versions/${songList.id}`}
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
								<EntryStatusMessage
									status={
										EntryStatus[songList.status as keyof typeof EntryStatus]
									}
								/>
							</>
						)}
					</>
				}
			>
				{songList.featuredCategory !== 'Nothing' &&
					songList.status === EntryStatus[EntryStatus.Draft] &&
					!songList.deleted && <DraftMessage section="" />}

				{songList.deleted && <DeletedBanner />}

				<div className="media">
					{smallThumbUrl && (
						<a className="pull-left" href={thumbUrl}>
							<img
								className="media-object"
								src={smallThumbUrl}
								alt="Thumb" /* TODO: localize */
							/>
						</a>
					)}
					<div className="media-body">
						{songList.eventDate && (
							<p>
								{t('ViewRes.SongList:Details.Date', {
									0: moment(songList.eventDate).format('l'),
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
									disabled={!loginManager.canEditTags}
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
											<a
												href={EntryUrlMapper.details(
													EntryType.ReleaseEvent,
													event.id,
													event.urlSlug,
												)}
											>
												{event.name}
											</a>{' '}
											<small>
												(
												{event.date ? (
													event.venue || event.venueName ? (
														<>
															{moment(event.date).format('l')}
															, <VenueLinkOrVenueName event={event} />
														</>
													) : (
														moment(event.date).format('l')
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
					<ButtonGroup className="songlist-mode-selection pull-left">
						<Button
							onClick={(): void =>
								runInAction(() => {
									songListStore.playlistMode = false;
								})
							}
							className={classNames(!songListStore.playlistMode && 'active')}
							href="#"
						>
							<i className="icon-th-list noMargin" />{' '}
							{t('ViewRes.SongList:Details.Details')}
						</Button>
						<Button
							onClick={(): void =>
								runInAction(() => {
									songListStore.playlistMode = true;
								})
							}
							className={classNames(songListStore.playlistMode && 'active')}
							href="#"
						>
							<i className="icon-list noMargin" />{' '}
							{t('ViewRes.SongList.Details:Playlist')}
						</Button>
					</ButtonGroup>
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
								..._.fromPairs(
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
								songListStore.showAdvancedFilters = !songListStore.showAdvancedFilters;
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
												songListStore.songType = eventKey;
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
						<PlayList
							playListStore={songListStore.playlistStore}
							pvPlayerStore={songListStore.pvPlayerStore}
						/>
					</div>
				) : (
					<div className={classNames(songListStore.loading && 'loading')}>
						<EntryCountBox pagingStore={songListStore.paging} />

						<ServerSidePaging pagingStore={songListStore.paging} />

						<table className="table table-striped">
							<tbody>
								{songListStore.page.map((item, index) => (
									<tr key={index}>
										<td style={{ width: '75px' }}>
											{item.song.thumbUrl && (
												<a
													href={EntryUrlMapper.details(
														EntryType.Song,
														item.song.id,
													)}
													title={item.song.additionalNames}
												>
													{/* eslint-disable-next-line jsx-a11y/alt-text */}
													<img
														src={item.song.thumbUrl}
														title="Cover picture" /* TODO: localize */
														className="coverPicThumb img-rounded"
														referrerPolicy="same-origin"
													/>
												</a>
											)}
										</td>
										<td>
											{item.song.previewStore &&
												item.song.previewStore.pvServices && (
													<div className="pull-right">
														<Button
															onClick={(): void =>
																item.song.previewStore?.togglePreview()
															}
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
											<a
												href={EntryUrlMapper.details(
													EntryType.Song,
													item.song.id,
												)}
												title={item.song.additionalNames}
											>
												{item.song.name}
											</a>
											{item.notes && (
												<>
													{' '}
													<span>
														(<span>{item.notes}</span>)
													</span>
												</>
											)}{' '}
											<SongTypeLabel
												songType={
													SongType[item.song.songType as keyof typeof SongType]
												}
											/>{' '}
											{songListStore.pvServiceIcons
												.getIconUrls(item.song.pvServices)
												.map((icon, index) => (
													<React.Fragment key={icon.service}>
														{index > 0 && ' '}
														{/* eslint-disable-next-line jsx-a11y/alt-text */}
														<img src={icon.url} title={icon.service} />
													</React.Fragment>
												))}{' '}
											<DraftIcon
												status={
													EntryStatus[
														item.song.status as keyof typeof EntryStatus
													]
												}
											/>
											<br />
											<small className="extraInfo">
												{item.song.artistString}
											</small>
											{item.song.previewStore &&
												item.song.previewStore.pvServices && (
													<PVPreviewKnockout
														previewStore={item.song.previewStore}
														getPvServiceIcons={
															songListStore.pvServiceIcons.getIconUrls
														}
													/>
												)}
										</td>
										{songListStore.showTags && (
											<td style={{ width: '33%' }}>
												{item.song.tags && item.song.tags.length > 0 && (
													<div>
														<i className="icon icon-tags" />{' '}
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
								))}
							</tbody>
						</table>

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
	const [model, setModel] = React.useState<
		{ songList: SongListContract; songListStore: SongListStore } | undefined
	>();

	const { id } = useParams();
	const navigate = useNavigate();

	React.useEffect(() => {
		songListRepo
			.getDetails({ id: Number(id) })
			.then((songList) =>
				setModel({
					songList: songList,
					songListStore: new SongListStore(
						vdb.values,
						urlMapper,
						songListRepo,
						songRepo,
						tagRepo,
						userRepo,
						artistRepo,
						songList.latestComments ?? [],
						songList.id,
						songList.tags ?? [],
						pvPlayersFactory,
						loginManager.canDeleteComments,
					),
				}),
			)
			.catch((error) => {
				if (error.response.status === 404) navigate('/Error/NotFound');
			});
	}, [id, navigate]);

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
