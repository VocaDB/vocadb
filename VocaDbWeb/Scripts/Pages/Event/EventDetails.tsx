import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import Dropdown from '@/Bootstrap/Dropdown';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { Layout } from '@/Components/Shared/Layout';
import { AlbumGrid } from '@/Components/Shared/Partials/Album/AlbumGrid';
import { ArtistLink } from '@/Components/Shared/Partials/Artist/ArtistLink';
import { LatestCommentsKnockout } from '@/Components/Shared/Partials/Comment/LatestCommentsKnockout';
import { DeletedBanner } from '@/Components/Shared/Partials/EntryDetails/DeletedBanner';
import { ExternalLinksList } from '@/Components/Shared/Partials/EntryDetails/ExternalLinksList';
import { ReportEntryPopupKnockout } from '@/Components/Shared/Partials/EntryDetails/ReportEntryPopupKnockout';
import { FormatMarkdown } from '@/Components/Shared/Partials/Html/FormatMarkdown';
import { EmbedPVPreview } from '@/Components/Shared/Partials/PV/EmbedPVPreview';
import { EmbedOpenStreetMap } from '@/Components/Shared/Partials/Shared/EmbedOpenStreetMap';
import { EntryStatusMessage } from '@/Components/Shared/Partials/Shared/EntryStatusMessage';
import { FormatPVLink } from '@/Components/Shared/Partials/Shared/FormatPVLink';
import { SongGrid } from '@/Components/Shared/Partials/Song/SongGrid';
import { SongIconLink } from '@/Components/Shared/Partials/Song/SongIconLink';
import { SongLink } from '@/Components/Shared/Partials/Song/SongLink';
import { TagLink } from '@/Components/Shared/Partials/Tag/TagLink';
import { TagList } from '@/Components/Shared/Partials/TagList';
import { TagsEdit } from '@/Components/Shared/Partials/TagsEdit';
import { IconAndNameLinkKnockout } from '@/Components/Shared/Partials/User/IconAndNameLinkKnockout';
import { ArtistForEventContract } from '@/DataContracts/ReleaseEvents/ArtistForEventContract';
import { ReleaseEventDetailsContract } from '@/DataContracts/ReleaseEvents/ReleaseEventDetailsContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { PVHelper } from '@/Helpers/PVHelper';
import { UrlHelper } from '@/Helpers/UrlHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryType } from '@/Models/EntryType';
import { ArtistEventRoles } from '@/Models/Events/ArtistEventRoles';
import { EventCategory } from '@/Models/Events/EventCategory';
import {
	EventReportType,
	eventReportTypesWithRequiredNotes,
} from '@/Models/Events/EventReportType';
import { ImageSize } from '@/Models/Images/ImageSize';
import { UserEventRelationshipType } from '@/Models/Users/UserEventRelationshipType';
import { useMutedUsers } from '@/MutedUsersContext';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import { ReleaseEventDetailsStore } from '@/Stores/ReleaseEvent/ReleaseEventDetailsStore';
import { SearchType } from '@/Stores/Search/SearchStore';
import { useVdb } from '@/VdbContext';
import { observer } from 'mobx-react-lite';
import moment from 'moment';
import NProgress from 'nprogress';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

interface ArtistListProps {
	artists: ArtistForEventContract[];
	showRoles?: boolean;
	showType?: boolean;
}

const ArtistList = React.memo(
	({
		artists,
		showRoles = false,
		showType = false,
	}: ArtistListProps): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Web.Resources.Domain.ReleaseEvents']);

		return (
			<>
				{artists.map((artist, index) => (
					<React.Fragment key={artist.id}>
						{index > 0 && ', '}
						{artist.artist ? (
							<ArtistLink artist={artist.artist} typeLabel={showType} />
						) : (
							artist.name
						)}
						{showRoles &&
							artist.effectiveRoles &&
							artist.effectiveRoles !==
								ArtistEventRoles[ArtistEventRoles.Default] && (
								<>
									{' '}
									<small className="muted">
										(
										{artist.effectiveRoles
											.split(',')
											.map((role) => role.trim())
											.map((role, index) => (
												<React.Fragment key={role}>
													{index > 0 && ', '}
													{t(
														`VocaDb.Web.Resources.Domain.ReleaseEvents:ArtistEventRoleNames.${role}`,
													)}
												</React.Fragment>
											))}
										)
									</small>
								</>
							)}
					</React.Fragment>
				))}
			</>
		);
	},
);

interface UserAttendingProps {
	user: UserApiContract;
}

const UserAttending = observer(
	({ user }: UserAttendingProps): React.ReactElement => {
		const mutedUsers = useMutedUsers();
		if (mutedUsers.includes(user.id)) return <></>;

		return (
			<li>
				<IconAndNameLinkKnockout user={user} />
			</li>
		);
	},
);

interface EventDetailsLayoutProps {
	event: ReleaseEventDetailsContract;
	releaseEventDetailsStore: ReleaseEventDetailsStore;
}

const EventDetailsLayout = observer(
	({
		event,
		releaseEventDetailsStore,
	}: EventDetailsLayoutProps): React.ReactElement => {
		const vdb = useVdb();
		const loginManager = useLoginManager();

		const { t, ready } = useTranslation([
			'ViewRes',
			'ViewRes.Event',
			'VocaDb.Web.Resources.Domain.ReleaseEvents',
		]);

		const subtitle =
			event.inheritedCategory === EventCategory.Unspecified ||
			event.inheritedCategory === EventCategory.Other
				? t('ViewRes.Event:Details.Event')
				: t(
						`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${event.inheritedCategory}`,
				  );

		const primaryPV = PVHelper.primaryPV(event.pvs, vdb.values.loggedUser);

		return (
			<Layout
				pageTitle={`${event.name} (${subtitle})`}
				ready={ready}
				title={event.name}
				subtitle={subtitle}
				parents={
					event.series ? (
						<>
							<Breadcrumb.Item
								linkAs={Link}
								linkProps={{
									to: '/Event',
								}}
								divider
							>
								{t(`ViewRes:Shared.ReleaseEvents`)}
							</Breadcrumb.Item>
							<Breadcrumb.Item
								linkAs={Link}
								linkProps={{
									to: EntryUrlMapper.details(
										EntryType.ReleaseEventSeries,
										event.series.id,
										event.series.urlSlug,
									),
								}}
							>
								{event.series.name}
							</Breadcrumb.Item>
						</>
					) : (
						<>
							<Breadcrumb.Item
								linkAs={Link}
								linkProps={{
									to: '/Event',
								}}
							>
								{t(`ViewRes:Shared.ReleaseEvents`)}
							</Breadcrumb.Item>
						</>
					)
				}
				toolbar={
					<>
						{primaryPV && (
							<div className="song-pv-player">
								<EmbedPVPreview
									entry={{
										...event,
										entryType: EntryType.ReleaseEvent,
									}}
									pv={primaryPV}
									allowInline
								/>
							</div>
						)}
						<JQueryUIButton
							as={Link}
							to={`/Event/Edit/${event.id}`}
							disabled={
								!loginManager.canEdit({
									...event,
									entryType: EntryType.ReleaseEvent,
								})
							}
							icons={{ primary: 'ui-icon-wrench' }}
						>
							{t('ViewRes:Shared.Edit')}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as={Link}
							to={`/Event/Versions/${event.id}`}
							icons={{ primary: 'ui-icon-clock' }}
						>
							{t('ViewRes:EntryDetails.ViewModifications')}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as={SafeAnchor}
							href="#"
							onClick={releaseEventDetailsStore.reportStore.show}
							icons={{ primary: 'ui-icon-alert' }}
						>
							{t('ViewRes:EntryDetails.ReportAnError')}
						</JQueryUIButton>{' '}
						<EntryStatusMessage status={event.status} />
					</>
				}
			>
				{event.deleted && <DeletedBanner />}

				<div className="well well-transparent">
					<div className="media">
						{event.mainPicture && (
							<a
								className="pull-left"
								href={UrlHelper.imageThumb(
									event.mainPicture,
									ImageSize.Original,
								)}
							>
								<img
									className="media-object"
									src={UrlHelper.imageThumb(
										event.mainPicture,
										ImageSize.SmallThumb,
									)}
									alt="Thumb" /* LOC */
								/>
							</a>
						)}

						<div className="media-body">
							<p>
								{t('ViewRes:Shared.Name')}: {event.name}
								{event.additionalNames && (
									<>
										{' '}
										<small className="extraInfo">
											({event.additionalNames})
										</small>
									</>
								)}
							</p>

							{event.date && (
								<p>
									{t('ViewRes.Event:Details.OccurrenceDate')}:{' '}
									{moment(event.date).format('l')}
									{event.endDate && event.endDate > event.date && (
										<> - {moment(event.endDate).format('l')}</>
									)}
								</p>
							)}

							{!event.venue && event.venueName && (
								<p>
									{t('ViewRes.Event:Details.Venue')}: {event.venueName}
								</p>
							)}

							{event.artists.length > 0 && (
								<p>
									Participating artists{/* LOC */}:{' '}
									<ArtistList
										artists={event.artists}
										showRoles={true}
										showType={true}
									/>
								</p>
							)}

							<p>
								{t('ViewRes.Event:Details.Category')}:{' '}
								{event.inheritedCategoryTag ? (
									<TagLink tag={event.inheritedCategoryTag} tooltip>
										{t(
											`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${event.inheritedCategory}`,
										)}
									</TagLink>
								) : (
									<a
										href={`/Tag/DetailsByEntryType?${qs.stringify({
											entryType: EntryType.ReleaseEvent,
											subType: event.inheritedCategory,
										})}`}
									>
										{t(
											`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${event.inheritedCategory}`,
										)}
									</a>
								)}
							</p>

							{event.description && <Markdown>{event.description}</Markdown>}

							{event.webLinks.length > 0 && (
								<>
									<ExternalLinksList
										webLinks={event.webLinks}
										showCategory={false}
									/>
									<br />
								</>
							)}

							<p>
								<div className="inline-block">{t('ViewRes:Shared.Tags')}:</div>
								{releaseEventDetailsStore.tagUsages.tagUsages.length > 0 && (
									<>
										{' '}
										<div className="entry-tag-usages inline-block">
											<TagList
												tagListStore={releaseEventDetailsStore.tagUsages}
											/>
										</div>
									</>
								)}
								<div>
									<JQueryUIButton
										as={SafeAnchor}
										className="inline-block"
										disabled={
											!loginManager.canEditTagsForEntry({
												...event,
												entryType: EntryType.ReleaseEvent,
											})
										}
										icons={{ primary: 'ui-icon-tag' }}
										onClick={releaseEventDetailsStore.tagsEditStore.show}
										href="#"
									>
										{t('ViewRes:EntryDetails.EditTags')}
									</JQueryUIButton>
									{event.canRemoveTagUsages /* TODO: Use LoginManager. */ && (
										<>
											{' '}
											<JQueryUIButton
												as={Link}
												to={`/Event/ManageTagUsages/${event.id}`}
												icons={{ primary: 'ui-icon-wrench' }}
											>
												{t('ViewRes:EntryDetails.ManageTags')}
											</JQueryUIButton>
										</>
									)}
								</div>
							</p>
						</div>
					</div>
				</div>

				{event.series && (
					<>
						<h3>
							{t('ViewRes.Event:Details.Series')}:{' '}
							<Link
								to={EntryUrlMapper.details(
									EntryType.ReleaseEventSeries,
									event.series.id,
									event.series.urlSlug,
								)}
								title={event.series.additionalNames}
							>
								{event.series.name}
							</Link>
						</h3>
						{event.series.description && (
							<FormatMarkdown text={event.series.description} />
						)}
						<ExternalLinksList
							webLinks={event.series.webLinks}
							showCategory={false}
						/>
					</>
				)}

				{event.songList && (
					<>
						<h3 className="withMargin">
							{t('ViewRes.Event:Details.SongList')}:{' '}
							<Link
								to={EntryUrlMapper.details(
									EntryType.SongList,
									event.songList.id,
								)}
							>
								{event.songList.name}
							</Link>
						</h3>
						{event.songListSongs && event.songListSongs.length > 0 && (
							<table className="table">
								{event.songListSongs.map((song) => (
									<tr key={song.order}>
										<td style={{ width: '50px' }}>
											<h1>{song.order}</h1>
										</td>
										<td style={{ width: '75px' }}>
											<SongIconLink song={song.song} />
										</td>
										<td>
											<SongLink song={song.song} />
											<br />
											<small className="extraInfo">
												{song.song.artistString}
											</small>
										</td>
									</tr>
								))}
							</table>
						)}
					</>
				)}

				{event.venue && (
					<>
						<h3 className="withMargin">
							{t('ViewRes.Event:Details.Venue')}:{' '}
							<Link
								to={EntryUrlMapper.details(EntryType.Venue, event.venue.id)}
								title={event.venue.additionalNames}
							>
								{event.venue.name}
							</Link>
						</h3>

						{event.venue.coordinates.hasValue && (
							<EmbedOpenStreetMap coordinates={event.venue.coordinates} />
						)}

						{event.venue.description && (
							<FormatMarkdown text={event.venue.description} />
						)}
						<ExternalLinksList
							webLinks={event.venue.webLinks}
							showCategory={false}
						/>
					</>
				)}

				{event.albums.length > 0 && (
					<>
						<h3 className="withMargin">
							{t('ViewRes.Event:Details.Albums')}{' '}
							<small>
								{t('ViewRes:EntryDetails.NumTotalParenthesis', {
									0: event.albums.length,
								})}
							</small>
						</h3>
						<AlbumGrid
							albums={event.albums}
							columns={2}
							displayRating={false}
							displayReleaseDate={false}
							displayType={true}
						/>
					</>
				)}

				{event.songs.length > 0 && (
					<>
						<h3 className="withMargin">
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.Song,
									eventId: event.id,
								})}`}
							>
								{t('ViewRes.Event:Details.Songs')}
							</Link>{' '}
							<small>
								{t('ViewRes:EntryDetails.NumTotalParenthesis', {
									0: event.songs.length,
								})}
							</small>
						</h3>
						<SongGrid
							songs={event.songs}
							columns={2}
							displayType={true}
							displayPublishDate={false}
						/>
					</>
				)}

				{event.pvs.length > 0 && (
					<>
						<h3 className="withMargin">{t('ViewRes.Event:Details.PVs')}</h3>
						{event.pvs.map((pv, index) => (
							<React.Fragment key={index}>
								{index > 0 && ', '}
								<FormatPVLink pv={pv} type={false} />
							</React.Fragment>
						))}
					</>
				)}

				{(loginManager.isLoggedIn ||
					releaseEventDetailsStore.usersAttending.length > 0) && (
					<h3 className="withMargin">{t('ViewRes.Event:Details.Attending')}</h3>
				)}

				{loginManager.isLoggedIn &&
					(releaseEventDetailsStore.hasEvent ? (
						<Dropdown as={ButtonGroup}>
							<Dropdown.Toggle>
								{releaseEventDetailsStore.isEventAttending && (
									<span>{t('ViewRes.Event:Details.UserAttending')}</span>
								)}
								{releaseEventDetailsStore.isEventInterested && (
									<span>{t('ViewRes.Event:Details.UserInterested')}</span>
								)}{' '}
								<span className="caret" />
							</Dropdown.Toggle>
							<Dropdown.Menu>
								<Dropdown.Item onClick={releaseEventDetailsStore.removeEvent}>
									{t('ViewRes.Event:Details.RemoveAttendance')}
								</Dropdown.Item>
								{releaseEventDetailsStore.isEventInterested && (
									<Dropdown.Item
										onClick={releaseEventDetailsStore.setEventAttending}
									>
										{t('ViewRes.Event:Details.UserAttending')}
									</Dropdown.Item>
								)}
								{releaseEventDetailsStore.isEventAttending && (
									<Dropdown.Item
										onClick={releaseEventDetailsStore.setEventInterested}
									>
										{t('ViewRes.Event:Details.UserInterested')}
									</Dropdown.Item>
								)}
							</Dropdown.Menu>
						</Dropdown>
					) : (
						<ButtonGroup>
							<Button onClick={releaseEventDetailsStore.setEventAttending}>
								{t('ViewRes.Event:Details.UserAttending')}
							</Button>
							<Button onClick={releaseEventDetailsStore.setEventInterested}>
								{t('ViewRes.Event:Details.UserInterested')}
							</Button>
						</ButtonGroup>
					))}

				{releaseEventDetailsStore.usersAttending.length > 0 && (
					<div className="withMargin">
						<ul>
							{releaseEventDetailsStore.usersAttending.map((user) => (
								<UserAttending user={user} key={user.id} />
							))}
						</ul>
					</div>
				)}

				<LatestCommentsKnockout
					editableCommentsStore={releaseEventDetailsStore.comments}
				/>

				<TagsEdit tagsEditStore={releaseEventDetailsStore.tagsEditStore} />

				<ReportEntryPopupKnockout
					reportEntryStore={releaseEventDetailsStore.reportStore}
					reportTypes={Object.values(EventReportType).map((r) => ({
						id: r,
						name: t(`VocaDb.Web.Resources.Domain:EntryReportTypeNames.${r}`),
						notesRequired: eventReportTypesWithRequiredNotes.includes(r),
					}))}
				/>
			</Layout>
		);
	},
);

const EventDetails = (): React.ReactElement => {
	const loginManager = useLoginManager();

	const [model, setModel] = React.useState<
		| {
				event: ReleaseEventDetailsContract;
				releaseEventDetailsStore: ReleaseEventDetailsStore;
		  }
		| undefined
	>();

	const { id } = useParams();

	React.useEffect(() => {
		NProgress.start();

		eventRepo
			.getDetails({ id: Number(id) })
			.then((event) => {
				setModel({
					event: event,
					releaseEventDetailsStore: new ReleaseEventDetailsStore(
						loginManager,
						httpClient,
						urlMapper,
						eventRepo,
						userRepo,
						event.latestComments,
						event.id,
						UserEventRelationshipType[
							event.eventAssociationType as keyof typeof UserEventRelationshipType
						],
						event.usersAttending,
						event.tags,
						loginManager.canDeleteComments,
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
	}, [loginManager, id]);

	return model ? (
		<EventDetailsLayout
			event={model.event}
			releaseEventDetailsStore={model.releaseEventDetailsStore}
		/>
	) : (
		<></>
	);
};

export default EventDetails;
