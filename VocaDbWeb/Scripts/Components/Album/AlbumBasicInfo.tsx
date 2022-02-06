import Carousel from '@Bootstrap/Carousel';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import AlbumDetailsForApi from '@DataContracts/Album/AlbumDetailsForApi';
import ArtistLinkContract from '@DataContracts/Song/ArtistLinkContract';
import DateTimeHelper from '@Helpers/DateTimeHelper';
import JQueryUIButton from '@JQueryUI/JQueryUIButton';
import JQueryUIDialog from '@JQueryUI/JQueryUIDialog';
import ContentFocus from '@Models/ContentFocus';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import LoginManager from '@Models/LoginManager';
import SongVoteRating from '@Models/SongVoteRating';
import SongType from '@Models/Songs/SongType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import AlbumDetailsStore from '@Stores/Album/AlbumDetailsStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

import { EventToolTip, TagToolTip } from '../KnockoutExtensions/EntryToolTip';
import MomentJsTimeAgo from '../KnockoutExtensions/MomentJsTimeAgo';
import CoverLink from '../Shared/Partials/Album/CoverLink';
import ArtistList, {
	ShowRolesMode,
} from '../Shared/Partials/Artist/ArtistList';
import LatestCommentsKnockout from '../Shared/Partials/Comment/LatestCommentsKnockout';
import EnglishTranslatedString from '../Shared/Partials/EnglishTranslatedString';
import ExternalLinksRows from '../Shared/Partials/EntryDetails/ExternalLinksRows';
import PersonalDescriptionMedia from '../Shared/Partials/EntryDetails/PersonalDescriptionMedia';
import FormatMarkdown from '../Shared/Partials/Html/FormatMarkdown';
import LanguageFlag from '../Shared/Partials/Html/LanguageFlag';
import EmbedPV from '../Shared/Partials/PV/EmbedPV';
import DraftIcon from '../Shared/Partials/Shared/DraftIcon';
import EntryPictureFileLink from '../Shared/Partials/Shared/EntryPictureFileLink';
import FormatPVLink from '../Shared/Partials/Shared/FormatPVLink';
import PVServiceIcons from '../Shared/Partials/Shared/PVServiceIcons';
import Stars from '../Shared/Partials/Shared/Stars';
import StarsMetaSpan from '../Shared/Partials/Shared/StarsMetaSpan';
import UniversalTimeLabel from '../Shared/Partials/Shared/UniversalTimeLabel';
import RatingIcon from '../Shared/Partials/Song/RatingIcon';
import SongLink from '../Shared/Partials/Song/SongLink';
import SongTypeLabel from '../Shared/Partials/Song/SongTypeLabel';
import TagList from '../Shared/Partials/TagList';
import ProfileIcon from '../Shared/Partials/User/ProfileIcon';
import UserLink from '../Shared/Partials/User/UserLink';
import { AlbumDetailsTabs } from './AlbumDetailsRoutes';

const loginManager = new LoginManager(vdb.values);

interface AlbumBasicInfoProps {
	model: AlbumDetailsForApi;
	albumDetailsStore: AlbumDetailsStore;
}

const AlbumBasicInfo = observer(
	({ model, albumDetailsStore }: AlbumBasicInfoProps): React.ReactElement => {
		const { t } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.Album',
			'VocaDb.Model.Resources.Albums',
		]);

		return (
			<AlbumDetailsTabs
				model={model}
				albumDetailsStore={albumDetailsStore}
				tab="basicInfo"
			>
				<div className="clearfix">
					{/* Album cover picture */}
					<div className="pull-left">
						<Carousel
							indicators={false}
							interval={null}
							slide={false}
							fade={false}
							controls={model.pictures.length > 0}
						>
							<Carousel.Item className="thumbItem">
								<CoverLink imageInfo={model.mainPicture} />
							</Carousel.Item>
							{model.pictures.map((picture, index) => (
								<Carousel.Item className="thumbItem" key={index}>
									<EntryPictureFileLink imageInfo={picture} />
									{picture.name && (
										<Carousel.Caption>
											<h4>{picture.name}</h4>
										</Carousel.Caption>
									)}
								</Carousel.Item>
							))}
						</Carousel>

						{model.ratingCount > 0 && (
							<div
								itemProp="aggregateRating"
								itemScope
								itemType="http://schema.org/AggregateRating"
							>
								<meta
									itemProp="ratingValue"
									content={`${model.ratingAverage}`}
								/>
								<StarsMetaSpan current={model.ratingAverage} max={5} />
								<br />(<span itemProp="ratingCount">
									{model.ratingCount}
								</span>{' '}
								{t('ViewRes.Album:Details.Ratings')})
							</div>
						)}
					</div>

					<table className="properties">
						<tbody>
							<tr>
								<td className="entry-field-label-col">
									{t('ViewRes:Shared.Name')}
								</td>
								<td itemProp="name">
									{model.name}
									<br />
									<span className="extraInfo">{model.additionalNames}</span>
								</td>
							</tr>

							{model.vocalists.length > 0 && (
								<tr>
									<td>{t('ViewRes.Album:Details.Vocalists')}</td>
									<td className="artistList">
										<ArtistList
											artists={model.vocalists as ArtistLinkContract[]}
											showRoles={ShowRolesMode.IfNotVocalist}
											showType={true}
											tooltip={true}
										/>
									</td>
								</tr>
							)}

							{model.subject.length > 0 && (
								<tr>
									<td>{t('ViewRes:EntryDetails.Subject')}</td>
									<td className="artistList">
										<ArtistList
											artists={model.subject as ArtistLinkContract[]}
											tooltip={true}
										/>
									</td>
								</tr>
							)}

							{model.producers.length > 0 && (
								<tr>
									<td>{t('ViewRes.Album:Details.Producers')}</td>
									<td className="artistList">
										<ArtistList
											artists={model.producers as ArtistLinkContract[]}
											showRoles={
												model.showProducerRoles
													? ShowRolesMode.IfNotDefault
													: ShowRolesMode.Never
											}
											tooltip={true}
										/>
									</td>
								</tr>
							)}

							{model.illustrators && model.illustrators.length > 0 && (
								<tr>
									<td>{t('ViewRes.Album:Details.Illustrators')}</td>
									<td className="artistList">
										<ArtistList
											artists={model.illustrators as ArtistLinkContract[]}
											tooltip={true}
										/>
									</td>
								</tr>
							)}

							{model.bands.length > 0 && (
								<tr>
									<td>{t('ViewRes.Album:Details.Band')}</td>
									<td className="artistList">
										<ArtistList
											artists={model.bands as ArtistLinkContract[]}
											tooltip={true}
										/>
									</td>
								</tr>
							)}

							{model.circles.length > 0 && (
								<tr>
									<td>{t('ViewRes.Album:Details.Circle')}</td>
									<td className="artistList">
										<ArtistList
											artists={model.circles as ArtistLinkContract[]}
											tooltip={true}
										/>
									</td>
								</tr>
							)}

							{model.labels.length > 0 && (
								<tr>
									<td>{t('ViewRes.Album:Details.Labels')}</td>
									<td className="artistList">
										<ArtistList
											artists={model.labels as ArtistLinkContract[]}
											tooltip={true}
										/>
									</td>
								</tr>
							)}

							{model.otherArtists.length > 0 && (
								<tr>
									<td>{t('ViewRes.Album:Details.OtherArtists')}</td>
									<td className="artistList">
										<ArtistList
											artists={model.otherArtists as ArtistLinkContract[]}
											tooltip={true}
										/>
									</td>
								</tr>
							)}

							{model.description.original && (
								<tr>
									<td>{t('ViewRes:Shared.Description')}</td>
									<td className="entry-description">
										<EnglishTranslatedString
											englishTranslatedStringStore={
												albumDetailsStore.description
											}
											string={model.description}
										/>
									</td>
								</tr>
							)}

							<tr>
								<td>{t('ViewRes:Shared.Type')}</td>
								<td>
									{model.discTypeTag ? (
										<TagToolTip
											as={Link}
											to={
												EntryUrlMapper.details_tag_contract(model.discTypeTag)!
											}
											id={model.discTypeTag.id}
										>
											{t(
												`VocaDb.Model.Resources.Albums:DiscTypeNames.${model.discType}`,
											)}
										</TagToolTip>
									) : (
										<a
											href={`/Tag/DetailsByEntryType?${qs.stringify({
												entryType: EntryType[EntryType.Album],
												subType: model.discType,
											})}`}
										>
											{t(
												`VocaDb.Model.Resources.Albums:DiscTypeNames.${model.discType}`,
											)}
										</a>
									)}
								</td>
							</tr>

							<tr>
								<td>{t('ViewRes:Shared.Tags')}</td>
								<td>
									{albumDetailsStore.tagUsages.tagUsages.length > 0 && (
										<div className="entry-tag-usages">
											<TagList tagListStore={albumDetailsStore.tagUsages} />
										</div>
									)}
									<div>
										<JQueryUIButton
											as={SafeAnchor}
											disabled={
												!loginManager.canEditTagsForEntry({
													...model,
													entryType: EntryType[EntryType.Album],
												})
											}
											icons={{ primary: 'ui-icon-tag' }}
											onClick={albumDetailsStore.tagsEditStore.show}
											href="#"
										>
											{t('ViewRes:EntryDetails.EditTags')}
										</JQueryUIButton>
										{model.canRemoveTagUsages /* TODO: Use LoginManager. */ && (
											<>
												{' '}
												<JQueryUIButton
													as="a"
													href={`/Album/ManageTagUsages/${model.id}`}
													icons={{ primary: 'ui-icon-wrench' }}
												>
													{t('ViewRes:EntryDetails.ManageTags')}
												</JQueryUIButton>
											</>
										)}
									</div>
								</td>
							</tr>

							{model.releaseDate && !model.releaseDate.isEmpty && (
								<tr>
									<td>{t('ViewRes.Album:Details.ReleaseDate')}</td>
									<td>
										{model.releaseDate.formatted}
										{model.releaseDateIsInThePast && (
											<>
												{' '}
												<small className="muted">
													{t('ViewRes.Album:Details.ReleaseDateIsInThePast')}
												</small>
											</>
										)}
										{model.releaseDateIsInTheNearFuture && (
											<>
												{' '}
												<small className="muted">
													{t(
														'ViewRes.Album:Details.ReleaseDateIsInTheNearFuture',
													)}
												</small>
											</>
										)}
										{model.releaseDateIsInTheFarFuture && (
											<>
												{' '}
												<small className="muted">
													{t(
														'ViewRes.Album:Details.ReleaseDateIsInTheFarFuture',
													)}
												</small>
											</>
										)}
										<br />
									</td>
								</tr>
							)}

							{model.catNum && (
								<tr>
									<td>{t('ViewRes.Album:Details.CatalogNumber')}</td>
									<td>{model.catNum}</td>
								</tr>
							)}

							{model.releaseEvent && (
								<tr>
									<td>{t('ViewRes.Album:Details.ReleaseEvent')}</td>
									<td>
										<EventToolTip
											as={Link}
											to={EntryUrlMapper.details(
												EntryType.ReleaseEvent,
												model.releaseEvent.id,
												model.releaseEvent.urlSlug,
											)}
											id={model.releaseEvent.id}
										>
											{model.releaseEvent.name}
										</EventToolTip>
									</td>
								</tr>
							)}

							{model.pvs.length > 0 && (
								<tr>
									<td>{t('ViewRes.Album:Details.PVs')}</td>
									<td>
										{model.pvs.map((pv) => (
											<React.Fragment key={pv.id}>
												<FormatPVLink pv={pv} type={false} />
												<br />
											</React.Fragment>
										))}
									</td>
								</tr>
							)}

							<ExternalLinksRows webLinks={model.webLinks} />

							<tr>
								<td>{t('ViewRes:EntryDetails.Stats')}</td>
								<td>
									<SafeAnchor
										href="#"
										id="statsLink"
										onClick={albumDetailsStore.getUsers}
									>
										{t('ViewRes:EntryDetails.Hits', { 0: model.hits })}.{' '}
										{t('ViewRes.Album:Details.OwnedBy', { 0: model.ownedBy })}.{' '}
										{t('ViewRes.Album:Details.WishlistedBy', {
											0: model.wishlistedBy,
										})}
										.
									</SafeAnchor>
								</td>
							</tr>

							<tr>
								<td>{t('ViewRes:EntryDetails.AdditionDate')}</td>
								<td>
									<UniversalTimeLabel dateTime={model.createDate} />
								</td>
							</tr>
						</tbody>
					</table>
				</div>

				{(model.discs.length > 0 ||
					model.contentFocus !== ContentFocus.Illustration) && (
					<>
						<h3>
							{t('ViewRes.Album:Details.Tracklist')}
							{model.totalLength !== 0 && (
								<>
									{' '}
									<small>
										(
										{t('ViewRes.Album:Details.TotalLength', {
											0: DateTimeHelper.formatFromSeconds(model.totalLength),
										})}
										)
									</small>
								</>
							)}
						</h3>
						<div className="tracklist">
							{model.discs.map((disc) => (
								<React.Fragment key={disc.discNumber}>
									{model.discs.length > 1 && (
										<h4>
											{t('ViewRes.Album:Details.Disc')} {disc.discNumber}
											{disc.name && ` - ${disc.name}`}
											{disc.totalLengthSeconds !== 0 && (
												<>
													{' '}
													<small>
														(
														{DateTimeHelper.formatFromSeconds(
															disc.totalLengthSeconds,
														)}
														)
													</small>
												</>
											)}
											{disc.isVideo && (
												<>
													{' '}
													<i
														className="icon-film"
														title={t('Resources:AlbumDiscMediaTypeNames.Video')}
													/>
												</>
											)}
										</h4>
									)}
									<ul>
										{disc.songs.map((song) => (
											<li className="tracklist-track" key={song.id}>
												<div className="tracklist-trackNumber">
													{song.trackNumber}
												</div>{' '}
												<div className="tracklist-trackTitle">
													{song.song ? (
														<>
															<SongLink
																song={song.song}
																albumId={model.id}
																tooltip={true}
															/>
															{song.song.lengthSeconds > 0 &&
																` (${DateTimeHelper.formatFromSeconds(
																	song.song.lengthSeconds,
																)})`}{' '}
															&nbsp;{' '}
															<DraftIcon
																status={
																	EntryStatus[
																		song.song.status as keyof typeof EntryStatus
																	]
																}
															/>
															{song.song.songType !==
																SongType[SongType.Original] &&
																song.song.songType !==
																	SongType[SongType.Unspecified] && (
																	<>
																		{' '}
																		<SongTypeLabel
																			songType={
																				SongType[
																					song.song
																						.songType as keyof typeof SongType
																				]
																			}
																		/>
																	</>
																)}{' '}
															<PVServiceIcons
																services={song.song.pvServices
																	.split(',')
																	.map((service) => service.trim())}
															/>{' '}
															<RatingIcon
																rating={
																	SongVoteRating[
																		song.rating as keyof typeof SongVoteRating
																	]
																}
															/>
															<br />
															<small className="muted">
																{song.song.artistString}
															</small>
														</>
													) : (
														song.name
													)}
												</div>
											</li>
										))}
									</ul>
								</React.Fragment>
							))}
						</div>
					</>
				)}

				{model.discs.length === 0 &&
					model.contentFocus !== ContentFocus.Illustration && (
						<p>{t('ViewRes.Album:Details.NoTracklist')}</p>
					)}

				{model.primaryPV && (
					<div className="song-pv-player">
						<EmbedPV pv={model.primaryPV} />
					</div>
				)}

				{(model.canEditPersonalDescription /* TODO: Use LoginManager. */ ||
					model.personalDescriptionText) && (
					<>
						<h3
							className="withMargin helpTip"
							title={t('ViewRes:EntryDetails.PersonalDescriptionHelp')}
						>
							{t('ViewRes:EntryDetails.PersonalDescription')}
						</h3>
						<PersonalDescriptionMedia
							personalDescription={albumDetailsStore.personalDescription}
							canEditPersonalDescription={model.canEditPersonalDescription}
						/>
					</>
				)}

				<LatestCommentsKnockout
					editableCommentsStore={albumDetailsStore.comments}
				/>

				<p>
					<Link
						to={`${EntryUrlMapper.details(
							EntryType.Album,
							model.id,
						)}/discussion`}
					>
						{t('ViewRes:EntryDetails.ViewAllComments')}
					</Link>
				</p>

				{model.latestReview && (
					<>
						<h3>{t('ViewRes.Album:Details.LatestReview')}</h3>

						<div className="media">
							<a
								className="pull-left"
								href={EntryUrlMapper.details_user_byName(
									model.latestReview.user.name,
								)}
							>
								<ProfileIcon
									url={model.latestReview.user.mainPicture?.urlThumb}
									size={70}
								/>
							</a>

							<div className="media-body">
								<div className="pull-right">
									<LanguageFlag
										languageCode={model.latestReview.languageCode}
									/>{' '}
									|{' '}
									<MomentJsTimeAgo as="span">
										{new Date(model.latestReview.date)}
									</MomentJsTimeAgo>
								</div>
								<h3 className="media-heading">
									<UserLink user={model.latestReview.user} />
								</h3>

								{model.latestReviewRatingScore > 0 && (
									<Stars current={model.latestReviewRatingScore} max={5} />
								)}

								<div>
									{model.latestReview.title && (
										<h4 className="album-review-title">
											{model.latestReview.title}
										</h4>
									)}

									<div>
										<FormatMarkdown text={model.latestReview.text} />
									</div>
								</div>
							</div>
						</div>

						<p className="withMargin">
							<Link
								to={`${EntryUrlMapper.details(
									EntryType.Album,
									model.id,
								)}/reviews`}
							>
								{t('ViewRes.Album:Details.ViewAllReviews', {
									0: model.reviewCount,
								})}
							</Link>
						</p>
					</>
				)}

				<JQueryUIDialog
					title={t('ViewRes.Album:Details.AlbumInCollections')}
					autoOpen={albumDetailsStore.userCollectionsPopupVisible}
					width={400}
					close={(): void =>
						runInAction(() => {
							albumDetailsStore.userCollectionsPopupVisible = false;
						})
					}
				>
					{albumDetailsStore.usersContent && (
						// HACK
						// TODO: Replace this with React
						<div
							dangerouslySetInnerHTML={{
								__html: albumDetailsStore.usersContent,
							}}
						/>
					)}
				</JQueryUIDialog>
			</AlbumDetailsTabs>
		);
	},
);

export default AlbumBasicInfo;
