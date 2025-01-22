import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { AlbumThumbs } from '@/Components/Shared/Partials/Album/AlbumThumbs';
import { ArtistGrid } from '@/Components/Shared/Partials/Artist/ArtistGrid';
import { ArtistLink } from '@/Components/Shared/Partials/Artist/ArtistLink';
import { ArtistLinkList } from '@/Components/Shared/Partials/Artist/ArtistLinkList';
import { ArtistTypeLabel } from '@/Components/Shared/Partials/Artist/ArtistTypeLabel';
import { LatestCommentsKnockout } from '@/Components/Shared/Partials/Comment/LatestCommentsKnockout';
import { EnglishTranslatedString } from '@/Components/Shared/Partials/EnglishTranslatedString';
import { ExternalLinksRows } from '@/Components/Shared/Partials/EntryDetails/ExternalLinksRows';
import { EventThumbs } from '@/Components/Shared/Partials/Shared/EventThumbs';
import { UniversalTimeLabel } from '@/Components/Shared/Partials/Shared/UniversalTimeLabel';
import { SongGrid } from '@/Components/Shared/Partials/Song/SongGrid';
import { TagLink } from '@/Components/Shared/Partials/Tag/TagLink';
import { TagList } from '@/Components/Shared/Partials/TagList';
import { TagsEdit } from '@/Components/Shared/Partials/TagsEdit';
import { UserIconLink_UserForApiContract } from '@/Components/Shared/Partials/User/UserIconLink_UserForApiContract';
import { useCultureCodes } from '@/CultureCodesContext';
import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { ArtistDetailsContract } from '@/DataContracts/Artist/ArtistDetailsContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryType } from '@/Models/EntryType';
import { ImageSize } from '@/Models/Images/ImageSize';
import { useMutedUsers } from '@/MutedUsersContext';
import { ArtistDetailsTabs } from '@/Pages/Artist/ArtistDetailsRoutes';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { ArtistDetailsStore } from '@/Stores/Artist/ArtistDetailsStore';
import { AlbumSortRule } from '@/Stores/Search/AlbumSearchStore';
import { EventSortRule } from '@/Stores/Search/EventSearchStore';
import { SearchType } from '@/Stores/Search/SearchStore';
import { SongSortRule } from '@/Stores/Search/SongSearchStore';
import { useVdb } from '@/VdbContext';
import dayjs from '@/dayjs';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface DataRowProps {
	label: string;
	content?: React.ReactNode;
}

const DataRow = React.memo(
	({ label, content }: DataRowProps): React.ReactElement => {
		return content ? (
			<tr>
				<td>{label}</td>
				<td>{content}</td>
			</tr>
		) : (
			<></>
		);
	},
);

interface ArtistListRowProps {
	label: string;
	artists: ArtistApiContract[];
	typeLabel: boolean;
	releaseYear?: boolean;
}

const ArtistListRow = React.memo(
	({
		label,
		artists,
		typeLabel,
		releaseYear = false,
	}: ArtistListRowProps): React.ReactElement => {
		return (
			<DataRow
				label={label}
				content={
					artists.length > 0 ? (
						<ArtistLinkList
							artists={artists}
							typeLabel={typeLabel}
							releaseYear={releaseYear}
							tooltip={true}
						/>
					) : undefined
				}
			/>
		);
	},
);

interface ArtistRowProps {
	label: string;
	artist?: ArtistApiContract;
	typeLabel: boolean;
}

const ArtistRow = React.memo(
	({ label, artist, typeLabel }: ArtistRowProps): React.ReactElement => {
		return (
			<DataRow
				label={label}
				content={
					artist ? (
						<ArtistLink artist={artist} typeLabel={typeLabel} tooltip={true} />
					) : undefined
				}
			/>
		);
	},
);

interface OwnedUserProps {
	user: UserApiContract;
}

const OwnedUser = observer(({ user }: OwnedUserProps): React.ReactElement => {
	const mutedUsers = useMutedUsers();
	if (mutedUsers.includes(user.id)) return <></>;

	return (
		<>
			{/* eslint-disable-next-line react/jsx-pascal-case */}
			<UserIconLink_UserForApiContract user={user} tooltip={true} />
			<br />
		</>
	);
});

interface ArtistBasicInfoProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistBasicInfo = observer(
	({
		artist,
		artistDetailsStore,
	}: ArtistBasicInfoProps): React.ReactElement => {
		const vdb = useVdb();
		const loginManager = useLoginManager();
		const { getCodeDescription } = useCultureCodes();

		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Artist',
			'ViewRes.Song',
			'VocaDb.Model.Resources',
		]);

		React.useEffect(() => {
			artistDetailsStore.loadHighcharts();
		}, [artistDetailsStore]);

		return (
			<ArtistDetailsTabs
				artist={artist}
				artistDetailsStore={artistDetailsStore}
				tab="basicInfo"
			>
				<div className="clearfix">
					{/* Artist picture */}
					<div className="pull-left entry-main-picture">
						<a href={`/Artist/Picture/${artist.id}?v=${artist.version}`}>
							<img
								src={UrlHelper.imageThumb(artist.mainPicture, ImageSize.Thumb)}
								alt={t('ViewRes.Artist:Details.ArtistPicture')}
								className="coverPic"
							/>
						</a>
					</div>

					<table className="properties">
						<tbody>
							<tr>
								<td className="entry-field-label-col">
									{t('ViewRes:Shared.ArtistName')}
								</td>
								<td>
									{artist.name}
									<br />
									<span className="extraInfo">{artist.additionalNames}</span>
								</td>
							</tr>
							{artist.description.original && (
								<tr>
									<td>{t('ViewRes:Shared.Description')}</td>
									<td className="entry-description">
										<EnglishTranslatedString
											englishTranslatedStringStore={
												artistDetailsStore.description
											}
											string={artist.description}
										/>
									</td>
								</tr>
							)}

							{artist.releaseDate && (
								<tr>
									<td>{t('ViewRes.Artist:Details.ReleaseDate')}</td>
									<td>
										{dayjs.utc(artist.releaseDate).format('l') /* REVIEW */}
									</td>
								</tr>
							)}

							<ArtistListRow
								label={t('ViewRes.Artist:Details.Illustrator')}
								artists={artist.illustrators}
								typeLabel={false}
							/>
							<ArtistListRow
								label={t('ViewRes.Artist:Details.IllustratorOf')}
								artists={artist.illustratorOf}
								typeLabel={true}
								releaseYear={true}
							/>
							<ArtistListRow
								label={t('ViewRes.Artist:Details.VoiceProvider')}
								artists={artist.voiceProviders}
								typeLabel={false}
							/>
							<ArtistListRow
								label={t('ViewRes.Artist:Details.VoiceProviderOf')}
								artists={artist.voicebanks}
								typeLabel={true}
								releaseYear={true}
							/>
							<ArtistListRow
								label={t('ViewRes.Artist:Details.ManagedBy')}
								artists={artist.managers}
								typeLabel={false}
							/>
							<ArtistListRow
								label={t('ViewRes.Artist:Details.ManagerOf')}
								artists={artist.managerOf}
								typeLabel={true}
								releaseYear={true}
							/>
							<ArtistListRow
								label={t('ViewRes.Artist:Details.CharacterDesigner')}
								artists={artist.characterDesigners}
								typeLabel={false}
							/>
							<ArtistListRow
								label={t('ViewRes.Artist:Details.CharacterDesignerOf')}
								artists={artist.characterDesignerOf}
								typeLabel={true}
							/>

							<tr>
								<td>{t('ViewRes:Shared.Type')}</td>
								<td>
									<ArtistTypeLabel artistType={artist.artistType} />{' '}
									{artist.artistTypeTag ? (
										<TagLink tag={artist.artistTypeTag} tooltip>
											{t(
												`VocaDb.Model.Resources:ArtistTypeNames.${artist.artistType}`,
											)}
										</TagLink>
									) : (
										<a
											href={`/Tag/DetailsByEntryType?${qs.stringify({
												entryType: EntryType.Artist,
												subType: artist.artistType,
											})}`}
										>
											{t(
												`VocaDb.Model.Resources:ArtistTypeNames.${artist.artistType}`,
											)}
										</a>
									)}
								</td>
							</tr>

							{artist.cultureCodes.length > 0 && (
								<tr>
									<td>{t('ViewRes:EntryDetails.Languages')}</td>
									<td>
										{artist.cultureCodes
											.map(
												(c) =>
													getCodeDescription(c)?.englishName ??
													t('ViewRes.Artist:Details.LyricsLanguageOther'),
											)
											.filter((l) => l !== '')
											.join(', ')}
									</td>
								</tr>
							)}

							<tr>
								<td>{t('ViewRes:Shared.Tags')}</td>
								<td>
									{artistDetailsStore.tagUsages.tagUsages.length > 0 && (
										<div className="entry-tag-usages">
											<TagList tagListStore={artistDetailsStore.tagUsages} />
										</div>
									)}
									<div>
										<JQueryUIButton
											as={SafeAnchor}
											disabled={
												!loginManager.canEditTagsForEntry({
													...artist,
													entryType: EntryType.Artist,
												})
											}
											icons={{ primary: 'ui-icon-tag' }}
											onClick={artistDetailsStore.tagsEditStore.show}
											href="#"
										>
											{t('ViewRes:EntryDetails.EditTags')}
										</JQueryUIButton>
										{artist.canRemoveTagUsages /* TODO: Use LoginManager. */ && (
											<>
												{' '}
												<JQueryUIButton
													as={Link}
													to={`/Artist/ManageTagUsages/${artist.id}`}
													icons={{ primary: 'ui-icon-wrench' }}
												>
													{t('ViewRes:EntryDetails.ManageTags')}
												</JQueryUIButton>
											</>
										)}
									</div>
								</td>
							</tr>

							<ExternalLinksRows webLinks={artist.webLinks} />

							{artist.ownerUsers.length > 0 && (
								<tr>
									<td>{vdb.resources.artist.authoredBy}</td>
									<td>
										{artist.ownerUsers.map((user) => (
											<OwnedUser user={user} key={user.id} />
										))}
									</td>
								</tr>
							)}

							{artist.baseVoicebank && (
								<tr>
									<td>
										<span>{t('ViewRes.Artist:Details.BaseVoicebank')}</span>
									</td>
									<td id="baseVoicebank">
										<ArtistLink
											artist={artist.baseVoicebank}
											releaseYear={true}
											tooltip={true}
											typeLabel={true}
										/>
									</td>
								</tr>
							)}

							{artist.childVoicebanks.length > 0 && (
								<tr>
									<td>
										<span>{t('ViewRes.Artist:Details.ChildVoicebanks')}</span>
									</td>
									<td id="childVoicebanks">
										{artist.childVoicebanks.map((artist, index) => (
											<React.Fragment key={artist.id}>
												{index > 0 && ', '}
												<ArtistLink
													artist={artist}
													releaseYear={true}
													tooltip={true}
													typeLabel={true}
												/>
											</React.Fragment>
										))}
									</td>
								</tr>
							)}

							{artist.groups.length > 0 && (
								<tr>
									<td>
										<span title={t('ViewRes.Artist:Details.GroupsHelp')}>
											{t('ViewRes.Artist:Details.Groups')}
										</span>
									</td>
									<td id="groups">
										{artist.groups.map((group, index) => (
											<React.Fragment key={group.id}>
												{index > 0 && ', '}
												<ArtistLink artist={group} tooltip={true} />
											</React.Fragment>
										))}
									</td>
								</tr>
							)}

							<tr>
								<td>{t('ViewRes:EntryDetails.Stats')}</td>
								<td>
									{t('ViewRes.Artist:Details.FollowCount', {
										0: artist.sharedStats.followerCount,
									})}
									{artist.sharedStats.ratedSongCount > 0 && (
										<>
											{' '}
											{t('ViewRes.Artist:Details.RatedSongs', {
												0: artist.sharedStats.ratedSongCount,
											})}
										</>
									)}
									{artist.sharedStats.ratedAlbumCount > 0 && (
										<>
											{' '}
											{t('ViewRes.Artist:Details.RatedAlbums', {
												0: artist.sharedStats.ratedAlbumCount,
											})}{' '}
											{t('ViewRes.Artist:Details.AverageAlbumRating', {
												0: artist.sharedStats.albumRatingAverage,
											})}
										</>
									)}
									{artist.personalStats &&
										artist.personalStats.songRatingCount > 0 && (
											<>
												{' '}
												<Link
													to={`${EntryUrlMapper.details_user_byName(
														loginManager.loggedUser?.name,
													)}/songs?${qs.stringify({
														artistId: artist.id,
													})}`}
												>
													{t('ViewRes.Artist:Details.YouHaveRatedSongs', {
														0: artist.personalStats.songRatingCount,
													})}
												</Link>
											</>
										)}
									{artist.advancedStats && (
										<p>
											{artist.advancedStats.topVocaloids.length > 0 && (
												<>
													{t('ViewRes.Artist:Details.MostlyUses')}{' '}
													<ArtistLinkList
														artists={artist.advancedStats.topVocaloids.map(
															(a) => a.data,
														)}
														typeLabel={true}
														tooltip={true}
													/>
												</>
											)}
											{artist.advancedStats.topLanguages.length > 0 && (
												<>
													<br />
													{t('ViewRes.Artist:Details.MostUsedLanguages')}{' '}
													{artist.advancedStats.topLanguages
														.map(
															(c) =>
																`${
																	getCodeDescription(c.data)?.englishName ??
																	t('ViewRes.Song:Details.LyricsLanguageOther')
																} (${c.count})`,
														)
														.join(', ')}
												</>
											)}
										</p>
									)}
								</td>
							</tr>

							<tr>
								<td>{t('ViewRes:EntryDetails.AdditionDate')}</td>
								<td>
									<UniversalTimeLabel dateTime={artist.createDate} />
								</td>
							</tr>
						</tbody>
					</table>
				</div>

				{artist.members.length > 0 && (
					<>
						<h3>{t('ViewRes.Artist:Details.Members')}</h3>
						{!artistDetailsStore.showAllMembers && (
							<div>
								<ArtistGrid
									artists={artist.members.take(6)}
									columns={3}
									displayType={true}
								/>
							</div>
						)}
						{artist.members.length > 6 && (
							<>
								{artistDetailsStore.showAllMembers ? (
									<div>
										<ArtistGrid
											artists={artist.members}
											columns={3}
											displayType={true}
										/>
									</div>
								) : (
									<SafeAnchor
										href="#"
										onClick={(): void =>
											runInAction(() => {
												artistDetailsStore.showAllMembers = true;
											})
										}
									>
										{t('ViewRes:Shared.ShowMore')}
									</SafeAnchor>
								)}
							</>
						)}
					</>
				)}

				{artist.latestAlbums.length > 0 && (
					<>
						<h3 className="withMargin">
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.Album,
									artistId: artist.id,
									sort: AlbumSortRule.AdditionDate,
								})}`}
							>
								{t('ViewRes.Artist:Details.RecentAlbums')}
							</Link>{' '}
							<small>
								{t('ViewRes:EntryDetails.NumTotalParenthesis', {
									0: artist.sharedStats.albumCount,
								})}
							</small>
						</h3>
						<div id="newAlbums">
							<AlbumThumbs albums={artist.latestAlbums} tooltip={true} />
						</div>
					</>
				)}

				{artist.topAlbums.length > 0 && (
					<>
						<h3 className="withMargin">
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.Album,
									artistId: artist.id,
									sort: AlbumSortRule.RatingTotal,
								})}`}
							>
								{t('ViewRes.Artist:Details.TopAlbums')}
							</Link>{' '}
							<small>
								(
								{t('ViewRes.Artist:Details.RatedAlbums', {
									0: artist.sharedStats.ratedAlbumCount,
								})}
								)
							</small>
						</h3>
						<div id="topAlbums">
							<AlbumThumbs albums={artist.topAlbums} tooltip={true} />
						</div>
					</>
				)}

				{artist.latestSongs.length > 0 && (
					<>
						<br />
						<h3>
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.Song,
									artistId: artist.id,
									sort: SongSortRule.PublishDate,
								})}`}
							>
								{t('ViewRes.Artist:Details.RecentSongs')}
							</Link>{' '}
							<small>
								(
								{t('ViewRes:EntryDetails.NumTotal', {
									0: artist.sharedStats.songCount,
								})}
								)
							</small>
						</h3>
						<SongGrid
							songs={artist.latestSongs}
							columns={2}
							displayType={true}
							displayPublishDate={true}
						/>
					</>
				)}

				{artist.topSongs.length > 0 && (
					<>
						<br />
						<h3>
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.Song,
									artistId: artist.id,
									sort: SongSortRule.RatingScore,
								})}`}
							>
								{t('ViewRes.Artist:Details.TopSongs')}
							</Link>{' '}
							<small>
								(
								{t('ViewRes.Artist:Details.RatedSongsTotal', {
									0: artist.sharedStats.ratedSongCount,
								})}
								)
							</small>
						</h3>
						<SongGrid
							songs={artist.topSongs}
							columns={2}
							displayType={true}
							displayPublishDate={true}
						/>
					</>
				)}

				{artist.latestEvents.length > 0 && (
					<>
						<h3 className="withMargin">
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.ReleaseEvent,
									artistId: artist.id,
									sort: EventSortRule.Date,
								})}`}
							>
								{t('ViewRes.Artist:Details.RecentEvents')}
							</Link>{' '}
							<small>
								(
								{t('ViewRes:EntryDetails.NumTotal', {
									0: artist.sharedStats.eventCount,
								})}
								)
							</small>
						</h3>
						<EventThumbs events={artist.latestEvents} />
					</>
				)}

				{artistDetailsStore.songsOverTimeChart && (
					<div>
						<h3 className="withMargin">
							{t('ViewRes.Artist:Details.SongsPerMonth')}
						</h3>
						<HighchartsReact
							highcharts={Highcharts}
							options={artistDetailsStore.songsOverTimeChart}
							immutable={true}
							containerProps={{
								style: {
									width: '100%',
									maxWidth: '800px',
									height: '300px',
								},
							}}
						/>
					</div>
				)}

				<LatestCommentsKnockout
					editableCommentsStore={artistDetailsStore.comments}
				/>

				<p>
					<Link
						to={`${EntryUrlMapper.details(
							EntryType.Artist,
							artist.id,
						)}/discussion`}
					>
						{t('ViewRes:EntryDetails.ViewAllComments')}
					</Link>
				</p>

				<TagsEdit tagsEditStore={artistDetailsStore.tagsEditStore} />
			</ArtistDetailsTabs>
		);
	},
);

export default ArtistBasicInfo;
