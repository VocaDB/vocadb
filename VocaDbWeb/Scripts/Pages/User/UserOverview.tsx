import SafeAnchor from '@Bootstrap/SafeAnchor';
import UserDetailsContract from '@DataContracts/User/UserDetailsContract';
import UserKnownLanguageContract, {
	UserLanguageProficiency,
} from '@DataContracts/User/UserKnownLanguageContract';
import LoginManager, { PermissionToken } from '@Models/LoginManager';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import UserDetailsStore from '@Stores/User/UserDetailsStore';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import { observer } from 'mobx-react-lite';
import moment from 'moment';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

import AlbumThumbs from '../../Components/Shared/Partials/Album/AlbumThumbs';
import ArtistGrid from '../../Components/Shared/Partials/Artist/ArtistGrid';
import EditableComments from '../../Components/Shared/Partials/Comment/EditableComments';
import ExternalLinksList from '../../Components/Shared/Partials/EntryDetails/ExternalLinksList';
import FormatMarkdown from '../../Components/Shared/Partials/Html/FormatMarkdown';
import LanguageFlag from '../../Components/Shared/Partials/Html/LanguageFlag';
import UniversalTimeLabel from '../../Components/Shared/Partials/Shared/UniversalTimeLabel';
import SongGrid from '../../Components/Shared/Partials/Song/SongGrid';
import TagLinkList from '../../Components/Shared/Partials/Tag/TagLinkList';
import ProfileIconKnockout from '../../Components/Shared/Partials/User/ProfileIconKnockout';
import { showErrorMessage, showSuccessMessage } from '../../Components/ui';
import { userLanguageCultures } from '../../Components/userLanguageCultures';
import { UserDetailsNav } from './UserDetailsRoutes';

const loginManager = new LoginManager(vdb.values);

interface AvatarImgProps {
	user: UserDetailsContract;
}

const AvatarImg = React.memo(
	({ user }: AvatarImgProps): React.ReactElement => {
		return (
			<ProfileIconKnockout
				icon={`${
					user.mainPicture?.urlThumb?.split('?')[0] ?? '/Content/unknown.png'
				}?s=120`}
				size={120}
			/>
		);
	},
);

interface AvatarProps {
	user: UserDetailsContract;
}

const Avatar = ({ user }: AvatarProps): React.ReactElement => {
	return loginManager.loggedUser &&
		loginManager.loggedUser.id === user.id &&
		loginManager.loggedUser.active ? (
		<a href="/User/MySettings#profile" id="avatar" className="user-avatar">
			<AvatarImg user={user} />
		</a>
	) : (
		<AvatarImg user={user} />
	);
};

interface PermissionTokenNameProps {
	token: PermissionToken;
}

const PermissionTokenName = React.memo(
	({ token }: PermissionTokenNameProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		const tokenName = Object.entries(PermissionToken)
			.filter(([_, value]) => value === token)
			.map(([key, _]) => key)[0];

		return t([`Resources:PermissionTokenNames.${tokenName}`, tokenName]);
	},
);

interface AllPermissionTokenNamesProps {
	tokens: PermissionToken[];
}

const AllPermissionTokenNames = React.memo(
	({ tokens }: AllPermissionTokenNamesProps): React.ReactElement => {
		return (
			<>
				{tokens.map((token, index) => (
					<React.Fragment key={token}>
						{index > 0 && ', '}
						<PermissionTokenName token={token} />
					</React.Fragment>
				))}
			</>
		);
	},
);

interface UserOverviewProps {
	user: UserDetailsContract;
	userDetailsStore: UserDetailsStore;
}

const UserOverview = observer(
	({ user, userDetailsStore }: UserOverviewProps): React.ReactElement => {
		const { t } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.User',
			'VocaDb.Web.Resources.Domain.Globalization',
			'VocaDb.Web.Resources.Domain.Users',
		]);

		const getLanguageName = React.useCallback(
			(lang: UserKnownLanguageContract): string => {
				if (!lang.cultureCode) {
					return t(
						'VocaDb.Web.Resources.Domain.Globalization:InterfaceLanguage.Other',
					);
				}

				const culture = userLanguageCultures[lang.cultureCode];
				return `${culture.nativeName} - ${culture.englishName}`;
			},
			[t],
		);

		const ownProfile =
			loginManager.loggedUser &&
			loginManager.loggedUser.id === user.id &&
			loginManager.loggedUser.active;
		const canSeeDetailedStats =
			ownProfile || !user.anonymousActivity || loginManager.canDisableUsers;

		const submitText = `${t('ViewRes.User:Details.Submissions')}: ${
			user.submitCount
		}`;
		const editText = `${t('ViewRes.User:Details.TotalEdits')}: ${
			user.editCount
		}`;
		const commentText = `${t('ViewRes.User:Details.CommentsMade')}: ${
			user.commentCount
		}`;

		React.useEffect(() => {
			userDetailsStore.loadHighcharts();
		}, [userDetailsStore]);

		return (
			<>
				<UserDetailsNav user={user} tab="overview" />

				<div className="row-fluid">
					<div className="span2 well well-transparent user-stats">
						<h4>{user.name}</h4>
						<Avatar user={user} />
						<br />
						<p className="withMargin">
							{t(`Resources:UserGroupNames.${user.groupId}`)}
							{user.designatedStaff && (
								<>
									{' '}
									<span className="label label-important">
										{t('ViewRes.User:Details.Staff')}
									</span>
								</>
							)}
							{user.verifiedArtist && (
								<>
									{' '}
									<span className="label label-success">
										{t('ViewRes.User:Details.VerifiedAccount')}
									</span>
								</>
							)}
							{user.isVeteran && (
								<>
									{' '}
									<span className="label label-success">
										{t('ViewRes.User:Details.Veteran', {
											0: vdb.values.siteName,
										})}
									</span>
								</>
							)}
							{user.customTitle && (
								<>
									{' '}
									<span className="label label-info">{user.customTitle}</span>
								</>
							)}
							{!user.active && (
								<>
									{' '}
									<span className="label">
										{t('ViewRes.User:Details.AccountDisabled')}
									</span>
								</>
							)}
							{user.emailVerified && (
								<>
									{' '}
									{/* eslint-disable-next-line jsx-a11y/alt-text */}
									<img
										src="/Content/Icons/tick.png"
										title={t('ViewRes.User:Details.VerifiedEmail')}
									/>
								</>
							)}
						</p>

						<h4 className="withMargin">{t('ViewRes.User:Details.StatsTab')}</h4>
						<span>
							{canSeeDetailedStats ? (
								<a
									href={`/User/EntryEdits/${user.id}?${qs.stringify({
										onlySubmissions: true,
									})}`}
								>
									{submitText}
								</a>
							) : (
								submitText
							)}
						</span>
						<br />
						<span>
							{canSeeDetailedStats ? (
								<a
									href={`/User/EntryEdits/${user.id}?${qs.stringify({
										onlySubmissions: false,
									})}`}
								>
									{editText}
								</a>
							) : (
								editText
							)}
						</span>
						<br />
						<span>
							{canSeeDetailedStats ? (
								<Link to={`/Comment?${qs.stringify({ userId: user.id })}`}>
									{commentText}
								</Link>
							) : (
								commentText
							)}
						</span>
						<br />
						<span>
							{t('ViewRes.User:Details.TagVotes')}: {user.tagVotes}
						</span>
						<br />
						<span>
							{t('ViewRes.User:Details.Power')}: {user.power} (
							{t('ViewRes.User:Details.Level')} {user.level})
						</span>

						<h4 className="withMargin">
							{t('ViewRes.User:Details.MemberSince')}
						</h4>
						{moment(user.createDate).format('l')}

						{user.oldUsernames.length > 0 && (
							<>
								<h4 className="withMargin">
									{t('ViewRes.User:Details.OldUsernames')}
								</h4>
								{user.oldUsernames.map((oldName, index) => (
									<React.Fragment key={index}>
										{index > 0 && ', '}
										{oldName.oldName}{' '}
										{t('ViewRes.User:Details.OldNameUntil', {
											0: moment(oldName.date).format('l'),
										})}
									</React.Fragment>
								))}
							</>
						)}

						{user.supporter && (
							<div className="withMargin media">
								{/* eslint-disable-next-line jsx-a11y/alt-text */}
								<img
									className="pull-left"
									style={{ marginTop: '2px' }}
									src="/Content/pixelart-miku.png"
								/>
								<div className="media-body">
									{t('ViewRes.User:Details.SupporterTitle', {
										0: vdb.values.siteName,
									})}
								</div>
							</div>
						)}
					</div>

					<div className="span8 well well-transparent">
						{user.aboutMe && (
							<>
								<h4>{t('ViewRes.User:Details.AboutMe')}</h4>
								<FormatMarkdown text={user.aboutMe} />
								<br />
							</>
						)}

						{user.knownLanguages.length > 0 && (
							<>
								<h4>{t('ViewRes.User:Details.LanguagesIKnow')}</h4>
								<ul className="user-known-languages">
									{user.knownLanguages.map((lang) => (
										<li key={lang.cultureCode}>
											<LanguageFlag languageCode={lang.cultureCode} />{' '}
											{getLanguageName(lang)}
											{lang.proficiency !== UserLanguageProficiency.Nothing && (
												<>
													{' '}
													(
													{t(
														`VocaDb.Web.Resources.Domain.Users:UserLanguageProficiencyNames.${lang.proficiency}`,
													)}
													)
												</>
											)}
										</li>
									))}
								</ul>
							</>
						)}

						{user.ownedArtistEntries.length > 0 && (
							<>
								<h4>{t('ViewRes.User:Details.VerifiedOwner')}</h4>
								<ArtistGrid
									artists={user.ownedArtistEntries.map((a) => a.artist)}
									columns={3}
									displayType={true}
								/>
								<br />
							</>
						)}

						{user.webLinks.length > 0 && (
							<>
								<h4 className="withMargin">
									{t('ViewRes:EntryDetails.ExternalLinks')}
								</h4>
								<ExternalLinksList webLinks={user.webLinks} />
								<br />
							</>
						)}

						{user.twitterName && (
							<>
								<h4>{t('ViewRes.User:Details.TwitterAccount')}</h4>
								<a
									href={`https://twitter.com/${user.twitterName}`}
									className="extLink"
								>
									{user.twitterName}
								</a>
								<br />
								<br />
							</>
						)}

						{user.location && (
							<>
								<h4>{t('ViewRes.User:Details.Location')}</h4>
								{user.location}
								<br />
								<br />
							</>
						)}

						{user.favoriteTags.length > 0 && (
							<>
								<h4>{t('ViewRes.User:Details.FavoriteTags')}</h4>
								<TagLinkList tagNames={user.favoriteTags} />
								<br />
								{userDetailsStore.ratingsByGenreChart && (
									<HighchartsReact
										highcharts={Highcharts}
										options={userDetailsStore.ratingsByGenreChart}
										immutable={true}
										containerProps={{
											style: {
												width: '400px',
												height: '200px',
											},
										}}
									/>
								)}
								<br />
							</>
						)}

						{ownProfile && !loginManager.canManageUserPermissions
							? user.additionalPermissions.length > 0 && (
									<>
										<h4>{t('ViewRes.User:Details.AdditionalPermissions')}</h4>
										<AllPermissionTokenNames
											tokens={user.additionalPermissions}
										/>
									</>
							  )
							: loginManager.canManageUserPermissions && (
									<>
										<h4>{t('ViewRes.User:Details.LastLogin')}</h4>
										<UniversalTimeLabel dateTime={user.lastLogin} /> (
										{user.lastLoginAddress}){' '}
										<a
											href={`http://www.geoiptool.com/?IP=${user.lastLoginAddress}`}
										>
											GeoIpTool
										</a>{' '}
										<SafeAnchor
											onClick={userDetailsStore.sfsCheckDialog.checkSFS}
											href="#"
										>
											StopForumSpam
										</SafeAnchor>{' '}
										<SafeAnchor
											onClick={async (): Promise<void> => {
												const result = await userDetailsStore.addBan({
													name: user.name,
												});

												if (result) {
													showSuccessMessage(
														'Added to ban list' /* TODO: localize */,
													);
												} else {
													showErrorMessage(
														'Already in the ban list' /* TODO: localize */,
													);
												}
											}}
											href="#"
										>
											Add to banned IPs{/* TODO: localize */}
										</SafeAnchor>
										{user.additionalPermissions.length > 0 && (
											<>
												<h4 className="withMargin">
													{t('ViewRes.User:Details.AdditionalPermissions')}
												</h4>
												<AllPermissionTokenNames
													tokens={user.additionalPermissions}
												/>
											</>
										)}
										{user.email && (
											<>
												<h4 className="withMargin">
													{t('ViewRes.User:Details.Email')}
												</h4>
												{user.email}
												{user.emailVerified && (
													<>
														{' '}
														{/* eslint-disable-next-line jsx-a11y/alt-text */}
														<img
															src="/Content/Icons/tick.png"
															title="Verified email" /* TODO: localize */
														/>
													</>
												)}
											</>
										)}
										<h4 className="withMargin">
											{t('ViewRes.User:Details.EffectivePermissions')}
										</h4>
										<AllPermissionTokenNames
											tokens={user.effectivePermissions}
										/>
									</>
							  )}
					</div>
				</div>

				<br />

				{user.favoriteAlbums.length > 0 && (
					<div className="well well-transparent">
						<div className="pull-right">
							<small>
								<Link
									to={`${EntryUrlMapper.details_user_byName(user.name)}/albums`}
								>
									{t('ViewRes:Shared.ShowMore')}
								</Link>
							</small>
						</div>
						<h3>
							<Link
								to={`${EntryUrlMapper.details_user_byName(user.name)}/albums`}
							>
								{t('ViewRes.User:Details.SomeAlbumsILike')}{' '}
								<small>
									(
									{t('ViewRes.User:Details.Total', {
										0: user.albumCollectionCount,
									})}
									)
								</small>
							</Link>
						</h3>
						<AlbumThumbs albums={user.favoriteAlbums} tooltip={true} />
					</div>
				)}

				{user.latestRatedSongs.length > 0 && (
					<div className="well well-transparent">
						<div className="pull-right">
							<small>
								<Link
									to={`${EntryUrlMapper.details_user_byName(user.name)}/songs`}
								>
									{t('ViewRes:Shared.ShowMore')}
								</Link>
							</small>
						</div>
						<h3>
							<Link
								to={`${EntryUrlMapper.details_user_byName(user.name)}/songs`}
							>
								{t('ViewRes.User:Details.SomeSongsILike')}{' '}
								<small>
									(
									{t('ViewRes.User:Details.Total', {
										0: user.favoriteSongCount,
									})}
									)
								</small>
							</Link>
						</h3>
						<SongGrid songs={user.latestRatedSongs} columns={3} />
					</div>
				)}

				{user.followedArtists.length > 0 && (
					<div className="well well-transparent">
						<div className="pull-right">
							<small>
								<Link
									to={`${EntryUrlMapper.details_user_byName(
										user.name,
									)}/artists`}
								>
									{t('ViewRes:Shared.ShowMore')}
								</Link>
							</small>
						</div>
						<h3>
							<Link
								to={`${EntryUrlMapper.details_user_byName(user.name)}/artists`}
							>
								{t('ViewRes.User:Details.SomeArtistsIFollow')}{' '}
								<small>
									({t('ViewRes.User:Details.Total', { 0: user.artistCount })})
								</small>
							</Link>
						</h3>
						<ArtistGrid artists={user.followedArtists} columns={3} />
					</div>
				)}

				<div className="well well-transparent">
					<div className="pull-right">
						<small>
							<Link
								to={`${EntryUrlMapper.details_user_byName(user.name)}/comments`}
							>
								{t('ViewRes:Shared.ShowMore')}
							</Link>
						</small>
					</div>
					<h3>
						<Link
							to={`${EntryUrlMapper.details_user_byName(user.name)}/comments`}
						>
							{t('ViewRes:EntryDetails.LatestComments')}
						</Link>
					</h3>
					<div>
						<EditableComments
							editableCommentsStore={userDetailsStore.comments}
							allowCreateComment={
								!user.standalone && loginManager.canCreateComments
							}
							well={false}
							comments={userDetailsStore.comments.topComments}
							newCommentRows={3}
							pagination={false}
						/>
						{!userDetailsStore.comments.comments.length && (
							<p>{t('ViewRes:EntryDetails.NoComments')}</p>
						)}
					</div>
				</div>
			</>
		);
	},
);

export default UserOverview;
