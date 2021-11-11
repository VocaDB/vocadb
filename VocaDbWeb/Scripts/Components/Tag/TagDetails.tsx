import Breadcrumb from '@Bootstrap/Breadcrumb';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import Layout from '@Components/Shared/Layout';
import AlbumGrid from '@Components/Shared/Partials/Album/AlbumGrid';
import ArtistGrid from '@Components/Shared/Partials/Artist/ArtistGrid';
import EditableComments from '@Components/Shared/Partials/Comment/EditableComments';
import EnglishTranslatedString from '@Components/Shared/Partials/EnglishTranslatedString';
import DeletedBanner from '@Components/Shared/Partials/EntryDetails/DeletedBanner';
import ExternalLinksList from '@Components/Shared/Partials/EntryDetails/ExternalLinksList';
import ReportEntryPopupKnockout from '@Components/Shared/Partials/EntryDetails/ReportEntryPopupKnockout';
import EntryStatusMessage from '@Components/Shared/Partials/Shared/EntryStatusMessage';
import EventSeriesThumbs from '@Components/Shared/Partials/Shared/EventSeriesThumbs';
import EventThumbs from '@Components/Shared/Partials/Shared/EventThumbs';
import ShowMore from '@Components/Shared/Partials/Shared/ShowMore';
import UniversalTimeLabel from '@Components/Shared/Partials/Shared/UniversalTimeLabel';
import SongGrid from '@Components/Shared/Partials/Song/SongGrid';
import TagList from '@Components/Shared/Partials/Tag/TagList';
import useVocaDbTitle from '@Components/useVocaDbTitle';
import EntryTypeAndSubTypeContract from '@DataContracts/EntryTypeAndSubTypeContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import TagDetailsContract from '@DataContracts/Tag/TagDetailsContract';
import UrlHelper from '@Helpers/UrlHelper';
import JQueryUIButton from '@JQueryUI/JQueryUIButton';
import JQueryUITab from '@JQueryUI/JQueryUITab';
import JQueryUITabs from '@JQueryUI/JQueryUITabs';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import ImageSize from '@Models/Images/ImageSize';
import LoginManager from '@Models/LoginManager';
import TagReportType, {
	reportTypesWithRequiredNotes,
} from '@Models/Tags/TagReportType';
import TagTargetTypes from '@Models/Tags/TagTargetTypes';
import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import functions from '@Shared/GlobalFunctions';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import { SearchType } from '@Stores/Search/SearchStore';
import TagDetailsStore from '@Stores/Tag/TagDetailsStore';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import _ from 'lodash';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { Helmet } from 'react-helmet-async';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);

interface NicoTagLinkProps {
	item: string;
}

const NicoTagLink = React.memo(
	({ item }: NicoTagLinkProps): React.ReactElement => {
		return (
			<a href={`http://www.nicovideo.jp/tag/${item}`} className="extLink">
				{item}
			</a>
		);
	},
);

const useEntrySubTypeName = (): ((
	fullEntryType: EntryTypeAndSubTypeContract,
) => string) => {
	const { t } = useTranslation([
		'VocaDb.Model.Resources',
		'VocaDb.Model.Resources.Albums',
		'VocaDb.Model.Resources.Songs',
		'VocaDb.Web.Resources.Domain.ReleaseEvents',
	]);

	return React.useCallback(
		(fullEntryType: EntryTypeAndSubTypeContract): string => {
			switch (EntryType[fullEntryType.entryType as keyof typeof EntryType]) {
				case EntryType.Album:
					return t(
						`VocaDb.Model.Resources.Albums:DiscTypeNames.${fullEntryType.subType}`,
					);

				case EntryType.Artist:
					return t(
						`VocaDb.Model.Resources:ArtistTypeNames.${fullEntryType.subType}`,
					);

				case EntryType.ReleaseEvent:
					return t(
						`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${fullEntryType.subType}`,
					);

				case EntryType.Song:
					return t(
						`VocaDb.Model.Resources.Songs:SongTypeNames.${fullEntryType.subType}`,
					);

				default:
					return '';
			}
		},
		[t],
	);
};

const tagLink = (tag: TagBaseContract): string => {
	var link = `<a href="${EntryUrlMapper.details_tag_contract(tag)}">${
		tag.name
	}</a>`;
	return link;
};

const tagLinks = (tagList: TagBaseContract[]): string => {
	var str = '';
	const links = _.map(tagList, (item) => tagLink(item));
	const tagsPerRow = 7;

	for (var i = 0; i < tagList.length; i += tagsPerRow) {
		str += _.reduce(
			_.take(_.drop(links, i), tagsPerRow),
			(list, item) => list + ', ' + item,
		);

		if (i < tagList.length + tagsPerRow) str += '<br/>';
	}

	return str;
};

interface HierarchyContainerProps {
	thisTag: string;
	parent?: TagBaseContract;
	siblings: TagBaseContract[];
	children: TagBaseContract[];
	hasMoreSiblings: boolean;
	hasMoreChildren: boolean;
}

const HierarchyContainer = React.memo(
	({
		thisTag,
		parent,
		siblings,
		children,
		hasMoreSiblings,
		hasMoreChildren,
	}: HierarchyContainerProps): React.ReactElement => {
		return (
			<HighchartsReact
				highcharts={Highcharts}
				options={{
					credits: { enabled: false },
					chart: {
						backgroundColor: null!,
						events: {
							load: function (this: any): void {
								// Draw the flow chart
								var ren = this.renderer,
									colors = Highcharts.getOptions().colors,
									downArrow = [
										'M',
										0,
										0,
										'L',
										0,
										40,
										'L',
										-5,
										35,
										'M',
										0,
										40,
										'L',
										5,
										35,
									],
									rightAndDownArrow = [
										'M',
										0,
										0,
										'L',
										70,
										0,
										'C',
										90,
										0,
										90,
										0,
										90,
										25,
										'L',
										90,
										80,
										'L',
										85,
										75,
										'M',
										90,
										80,
										'L',
										95,
										75,
									];

								var y = 10;

								if (parent) {
									var parentLab = ren
										.label('Parent tag:<br/>' + tagLink(parent), 10, y)
										.attr({
											fill: colors![0],
											stroke: 'white',
											'stroke-width': 2,
											padding: 5,
											r: 5,
										})
										.css({
											color: 'white',
										})
										.add()
										.shadow(true);

									// Arrow from parent to this tag
									ren
										.path(downArrow)
										.translate(50, y + 60)
										.attr({
											'stroke-width': 2,
											stroke: colors![3],
										})
										.add();

									if (siblings && siblings.length) {
										// Arrow from parent to siblings
										ren
											.path(rightAndDownArrow)
											.translate(
												parentLab.getBBox().x + parentLab.getBBox().width + 20,
												y + 20,
											)
											.attr({
												'stroke-width': 2,
												stroke: colors![3],
											})
											.add();

										const siblingsText =
											'Siblings:<br/>' +
											tagLinks(siblings) +
											(hasMoreSiblings ? ' (+ more)' : '');

										ren
											.label(siblingsText, 150, y + 115)
											.attr({
												fill: colors![4],
												stroke: 'white',
												'stroke-width': 2,
												padding: 5,
												r: 5,
											})
											.css({
												color: 'white',
											})
											.add()
											.shadow(true);
									}

									y += 115;
								}

								ren
									.label('This tag:<br />' + thisTag, 10, y)
									.attr({
										fill: colors![1],
										stroke: 'white',
										'stroke-width': 2,
										padding: 5,
										r: 5,
									})
									.css({
										color: 'white',
									})
									.add()
									.shadow(true);

								if (children && children.length) {
									// Arrow from this to children
									ren
										.path(downArrow)
										.translate(50, y + 60)
										.attr({
											'stroke-width': 2,
											stroke: colors![3],
										})
										.add();

									const childrenText =
										'Children:<br/>' +
										tagLinks(children) +
										(hasMoreChildren ? ' (+ more)' : '');

									ren
										.label(childrenText, 10, y + 115)
										.attr({
											fill: colors![4],
											stroke: 'white',
											'stroke-width': 2,
											padding: 5,
											r: 5,
										})
										.css({
											color: 'white',
										})
										.add()
										.shadow(true);
								}
							},
						},
					},
					title: {
						text: null!,
					},
				}}
				immutable={true}
				containerProps={{
					style: {
						width: '1000px',
						height: `${
							75 + (parent ? 125 : 0) + (_.some(children) ? 125 : 0)
						}px`,
					},
				}}
			/>
		);
	},
);

interface TagDetailsLayoutProps {
	tag: TagDetailsContract;
	tagDetailsStore: TagDetailsStore;
}

const TagDetailsLayout = observer(
	({ tag, tagDetailsStore }: TagDetailsLayoutProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Tag',
			'VocaDb.Web.Resources.Domain',
		]);

		useVocaDbTitle(tag.name, true);

		const [tab, setTab] = React.useState('latestComments');

		React.useEffect(() => {
			if (tab === 'discussion') tagDetailsStore.comments.initComments();
		}, [tab, tagDetailsStore]);

		const entrySubTypeName = useEntrySubTypeName();

		return (
			<Layout
				// TODO: globalSearchType
				title={tag.name}
				subtitle={t('ViewRes.Tag:Details.Tag')}
				// TODO: canonicalUrl
				parents={
					<>
						<Breadcrumb.Item linkAs="a" href="/Tag">
							{t('ViewRes:Shared.Tags')}
						</Breadcrumb.Item>
					</>
				}
				// TODO: robots
				toolbar={
					<>
						{tagDetailsStore.isFollowed ? (
							<JQueryUIButton
								as={SafeAnchor}
								href="#"
								onClick={tagDetailsStore.unfollowTag}
								icons={{ primary: 'ui-icon-close' }}
							>
								{t('ViewRes.Tag:Details.UnfollowTag')}
							</JQueryUIButton>
						) : (
							<JQueryUIButton
								as={SafeAnchor}
								href="#"
								onClick={tagDetailsStore.followTag}
								icons={{ primary: 'ui-icon-plus' }}
								disabled={!loginManager.isLoggedIn}
							>
								{t('ViewRes.Tag:Details.FollowTag')}
							</JQueryUIButton>
						)}{' '}
						<JQueryUIButton
							as="a"
							href={`/Tag/Edit/${tag.id}`}
							disabled={
								!loginManager.canEdit({
									...tag,
									entryType: EntryType[EntryType.Tag],
								})
							}
							icons={{ primary: 'ui-icon-wrench' }}
						>
							{t('ViewRes:Shared.Edit')}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as="a"
							href={`/Tag/Versions/${tag.id}`}
							icons={{ primary: 'ui-icon-clock' }}
						>
							{t('ViewRes:EntryDetails.ViewModifications')}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as={SafeAnchor}
							href="#"
							onClick={tagDetailsStore.reportStore.show}
							icons={{ primary: 'ui-icon-alert' }}
						>
							{t('ViewRes:EntryDetails.ReportAnError')}
						</JQueryUIButton>{' '}
						<EntryStatusMessage
							status={EntryStatus[tag.status as keyof typeof EntryStatus]}
						/>
					</>
				}
			>
				<Helmet>
					<link
						rel="canonical"
						href={functions.mergeUrls(
							vdb.values.hostAddress,
							EntryUrlMapper.details_tag_contract(tag)!,
						)}
					/>
				</Helmet>

				{tag.deleted && <DeletedBanner />}

				<div className="well well-transparent">
					<div className="media">
						{tag.mainPicture && (
							<a
								className="pull-left"
								href={UrlHelper.getSmallestThumb(
									tag.mainPicture,
									ImageSize.Original,
								)}
							>
								<img
									className="media-object"
									src={UrlHelper.getSmallestThumb(
										tag.mainPicture,
										ImageSize.SmallThumb,
									)}
									alt="Thumb" /* TODO: localize */
								/>
							</a>
						)}

						<div className="media-body">
							{tag.description.original && (
								<div className="entry-description">
									<EnglishTranslatedString
										englishTranslatedStringStore={tagDetailsStore.description}
										string={tag.description}
										maxLength={2100}
										summaryLength={2000}
									/>
								</div>
							)}

							{tag.relatedTags.length > 0 && (
								<p>
									{t('ViewRes.Tag:Details.RelatedTags')}:{' '}
									<TagList tagNames={tag.relatedTags} tooltip={true} />
								</p>
							)}

							{tag.webLinks.length > 0 && (
								<>
									<ExternalLinksList
										webLinks={tag.webLinks}
										showCategory={false}
									/>
									<br />
								</>
							)}

							{tag.mappedNicoTags.length > 0 && (
								<p>
									{t('ViewRes.Tag:Details.MappedTags')}:{' '}
									{tag.mappedNicoTags.map((mappedNicoTag, index) => (
										<React.Fragment key={mappedNicoTag}>
											{index > 0 && ', '}
											<NicoTagLink item={mappedNicoTag} />
										</React.Fragment>
									))}
								</p>
							)}

							{tag.categoryName && (
								<p>
									{t('ViewRes.Tag:Details.Category')}: {tag.categoryName}
								</p>
							)}

							{tag.translations && (
								<p>
									{t('ViewRes.Tag:Details.Translations')}: {tag.translations}
								</p>
							)}

							{tag.additionalNames && (
								<p>
									{t('ViewRes.Tag:Details.Aliases')}: {tag.additionalNames}
								</p>
							)}

							{(tag.parent ||
								tag.siblings.length > 0 ||
								tag.children.length > 0) && (
								<>
									<HierarchyContainer
										thisTag={tag.name}
										parent={tag.parent}
										siblings={_.slice(tag.siblings, 0, 20)}
										children={_.slice(tag.children, 0, 20)}
										hasMoreSiblings={tag.siblings.length > 20}
										hasMoreChildren={tag.children.length > 20}
									/>
									<br />
								</>
							)}

							{tag.targets !== TagTargetTypes.Nothing &&
								tag.targets !== TagTargetTypes.All && (
									<p>
										{t('ViewRes.Tag:Details.ValidFor')}:{' '}
										{Object.values(EntryType)
											.filter(
												(e) =>
													e !== EntryType.Undefined &&
													(tag.targets & Number(e)) !== 0,
											)
											.map((e) =>
												t(
													`VocaDb.Web.Resources.Domain:EntryTypeNames.${
														EntryType[e as keyof typeof EntryType]
													}`,
												),
											)
											.join(', ')}
									</p>
								)}

							{tag.relatedEntryType.hasValue && (
								<p>
									{t('ViewRes.Tag:Details.AssociatedEntryType')}:{' '}
									<Link to={EntryUrlMapper.index(tag.relatedEntryType)}>
										{t(
											`VocaDb.Web.Resources.Domain:EntryTypeNames.${tag.relatedEntryType.entryType}`,
										)}
										{tag.relatedEntryType.hasSubType && (
											<> ({entrySubTypeName(tag.relatedEntryType)})</>
										)}
									</Link>
								</p>
							)}

							<p>
								{t('ViewRes.Tag:Details.FollowCount', {
									0: tag.stats.followerCount,
								})}
							</p>

							<p>
								{t('ViewRes:EntryDetails.AdditionDate')}:{' '}
								<UniversalTimeLabel dateTime={tag.createDate} />
							</p>

							<br />

							<div>
								<Link to={`/Search?${qs.stringify({ tagId: tag.id })}`}>
									{t('ViewRes.Tag:Details.AllEntries')} ({tag.allUsageCount})
								</Link>
							</div>

							{tag.stats.artistCount > 0 && (
								<div>
									<Link
										to={`/Search?${qs.stringify({
											searchType: SearchType.Artist,
											tagId: tag.id,
										})}`}
									>
										{t('ViewRes.Tag:Details.AllArtists')} (
										{tag.stats.artistCount})
									</Link>
								</div>
							)}

							{tag.stats.albumCount > 0 && (
								<div>
									<Link
										to={`/Search?${qs.stringify({
											searchType: SearchType.Album,
											tagId: tag.id,
										})}`}
									>
										{t('ViewRes.Tag:Details.AllAlbums')} ({tag.stats.albumCount}
										)
									</Link>
								</div>
							)}

							{tag.stats.songCount > 0 && (
								<div>
									<Link
										to={`/Search?${qs.stringify({
											searchType: SearchType.Song,
											tagId: tag.id,
										})}`}
									>
										{t('ViewRes.Tag:Details.AllSongs')} ({tag.stats.songCount})
									</Link>
								</div>
							)}

							{tag.stats.eventCount > 0 && (
								<div>
									<Link
										to={`/Search?${qs.stringify({
											searchType: SearchType.ReleaseEvent,
											tagId: tag.id,
										})}`}
									>
										{t('ViewRes.Tag:Details.AllEvents')} ({tag.stats.eventCount}
										)
									</Link>
								</div>
							)}

							{tag.stats.songListCount > 0 && (
								<div>
									<Link
										to={`/SongList/Featured?${qs.stringify({
											tagId: tag.id,
										})}`}
									>
										{t('ViewRes.Tag:Details.AllSongLists')} (
										{tag.stats.songListCount})
									</Link>
								</div>
							)}
						</div>
					</div>
				</div>

				{tag.stats.artists.length > 0 && (
					<div className="well well-transparent">
						<ShowMore
							as={Link}
							to={`/Search?${qs.stringify({
								searchType: SearchType.Artist,
								tagId: tag.id,
							})}`}
						/>
						<h3>
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.Artist,
									tagId: tag.id,
								})}`}
							>
								{t('ViewRes.Tag:Details.TopArtists')}{' '}
								<small>
									({tag.stats.artistCount} {t('ViewRes:Shared.Total')})
								</small>
							</Link>
						</h3>
						<ArtistGrid
							artists={tag.stats.artists}
							columns={2}
							displayType={true}
						/>
					</div>
				)}

				{tag.stats.albums.length > 0 && (
					<div className="well well-transparent">
						<ShowMore
							as={Link}
							to={`/Search?${qs.stringify({
								searchType: SearchType.Album,
								tagId: tag.id,
							})}`}
						/>
						<h3>
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.Album,
									tagId: tag.id,
								})}`}
							>
								{t('ViewRes.Tag:Details.TopAlbums')}{' '}
								<small>
									({tag.stats.albumCount} {t('ViewRes:Shared.Total')})
								</small>
							</Link>
						</h3>
						<AlbumGrid
							albums={tag.stats.albums}
							columns={2}
							displayRating={false}
							displayReleaseDate={false}
							displayType={true}
						/>
					</div>
				)}

				{tag.stats.songs.length > 0 && (
					<div className="well well-transparent">
						<ShowMore
							as={Link}
							to={`/Search?${qs.stringify({
								searchType: SearchType.Song,
								tagId: tag.id,
							})}`}
						/>
						<h3>
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.Song,
									tagId: tag.id,
								})}`}
							>
								{t('ViewRes.Tag:Details.TopSongs')}{' '}
								<small>
									({tag.stats.songCount} {t('ViewRes:Shared.Total')})
								</small>
							</Link>
						</h3>
						<SongGrid songs={tag.stats.songs} columns={2} displayType={true} />
					</div>
				)}

				{tag.stats.eventSeries.length > 0 && (
					<div className="well well-transparent">
						<ShowMore
							as={Link}
							to={`/Search?${qs.stringify({
								searchType: SearchType.ReleaseEvent,
								tagId: tag.id,
							})}`}
						/>
						<h3>
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.ReleaseEvent,
									tagId: tag.id,
								})}`}
							>
								{t('ViewRes.Tag:Details.EventSeries')}
							</Link>
						</h3>
						<EventSeriesThumbs events={tag.stats.eventSeries} />
					</div>
				)}

				{tag.stats.events.length > 0 && (
					<div className="well well-transparent">
						<ShowMore
							as={Link}
							to={`/Search?${qs.stringify({
								searchType: SearchType.ReleaseEvent,
								tagId: tag.id,
							})}`}
						/>
						<h3>
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.ReleaseEvent,
									tagId: tag.id,
								})}`}
							>
								{t('ViewRes.Tag:Details.TopEvents')}{' '}
								<small>
									({tag.stats.eventCount} {t('ViewRes:Shared.Total')})
								</small>
							</Link>
						</h3>
						<EventThumbs
							events={tag.stats.events}
							imageSize={ImageSize.TinyThumb}
						/>
					</div>
				)}

				{(tag.siblings.length > 0 || tag.children.length > 0) && (
					<div className="well well-transparent">
						{tag.siblings.length > 0 && (
							<p>
								{t('ViewRes.Tag:Details.Siblings')}:{' '}
								<TagList tagNames={tag.siblings} />
							</p>
						)}
						{tag.children.length > 0 && (
							<p>
								{t('ViewRes.Tag:Details.Children')}:{' '}
								<TagList tagNames={tag.children} />
							</p>
						)}
					</div>
				)}

				<JQueryUITabs activeKey={tab} onSelect={(k): void => setTab(k!)}>
					<JQueryUITab
						eventKey="latestComments"
						title={t('ViewRes:EntryDetails.LatestComments')}
					>
						{tagDetailsStore.comments.comments.length > 0 ? (
							<EditableComments
								editableCommentsStore={tagDetailsStore.comments}
								allowCreateComment={loginManager.canCreateComments}
								well={false}
								comments={tagDetailsStore.comments.topComments}
								newCommentRows={3}
								pagination={false}
							/>
						) : (
							<p>{t('ViewRes:EntryDetails.NoComments')}</p>
						)}
						<p>
							<SafeAnchor href="#" onClick={(): void => setTab('discussion')}>
								{t('ViewRes:EntryDetails.ViewAllComments')}
							</SafeAnchor>
						</p>
					</JQueryUITab>

					<JQueryUITab
						eventKey="discussion"
						title={`${t('ViewRes:EntryDetails.DiscussionTab')} (${
							tag.commentCount
						})`}
					>
						<EditableComments
							editableCommentsStore={tagDetailsStore.comments}
							allowCreateComment={loginManager.canCreateComments}
							well={false}
							comments={tagDetailsStore.comments.pageOfComments}
						/>
					</JQueryUITab>
				</JQueryUITabs>

				<ReportEntryPopupKnockout
					reportEntryStore={tagDetailsStore.reportStore}
					reportTypes={Object.values(TagReportType).map((r) => ({
						id: r,
						name: t(`VocaDb.Web.Resources.Domain:EntryReportTypeNames.${r}`),
						notesRequired: reportTypesWithRequiredNotes.includes(r),
					}))}
				/>
			</Layout>
		);
	},
);

const TagDetails = (): React.ReactElement => {
	const [model, setModel] = React.useState<
		{ tag: TagDetailsContract; tagDetailsStore: TagDetailsStore } | undefined
	>();

	const { id } = useParams();
	const navigate = useNavigate();

	React.useEffect(() => {
		tagRepo
			.getDetails({ id: Number(id) })
			.then((tag) => {
				setModel({
					tag: tag,
					tagDetailsStore: new TagDetailsStore(
						loginManager,
						tagRepo,
						userRepo,
						tag.latestComments,
						tag.id,
						loginManager.canDeleteComments,
						(vdb.values.languagePreference ===
							ContentLanguagePreference.English ||
							vdb.values.languagePreference ===
								ContentLanguagePreference.Romaji) &&
							!!tag.description.english,
						tag.isFollowing,
					),
				});
			})
			.catch((error) => {
				if (error.response.status === 404) navigate('/Error/NotFound');
			});
	}, [id, navigate]);

	return model ? (
		<TagDetailsLayout tag={model.tag} tagDetailsStore={model.tagDetailsStore} />
	) : (
		<></>
	);
};

export default TagDetails;
