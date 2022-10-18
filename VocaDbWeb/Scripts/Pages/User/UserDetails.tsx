import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { EntryDeletePopupBase } from '@/Components/Shared/Partials/EntryDetails/EntryDeletePopupBase';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { UserDetailsContract } from '@/DataContracts/User/UserDetailsContract';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import JQueryUIDialog from '@/JQueryUI/JQueryUIDialog';
import { LoginManager } from '@/Models/LoginManager';
import { UserGroup } from '@/Models/Users/UserGroup';
import UserDetailsRoutes from '@/Pages/User/UserDetailsRoutes';
import { AdminRepository } from '@/Repositories/AdminRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { PVPlayersFactory } from '@/Stores/PVs/PVPlayersFactory';
import { AlbumCollectionStore } from '@/Stores/User/AlbumCollectionStore';
import { FollowedArtistsStore } from '@/Stores/User/FollowedArtistsStore';
import { RatedSongsSearchStore } from '@/Stores/User/RatedSongsSearchStore';
import { UserDetailsStore } from '@/Stores/User/UserDetailsStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import NProgress from 'nprogress';
import qs from 'qs';
import React from 'react';
import { Trans, useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

import '../../../wwwroot/Content/Styles/songlist.less';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const userRepo = new UserRepository(httpClient, urlMapper);
const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);
const songRepo = new SongRepository(httpClient, vdb.values.baseAddress);
const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);
const adminRepo = new AdminRepository(httpClient, urlMapper);

const pvPlayersFactory = new PVPlayersFactory();

interface UserDetailsLayoutProps {
	user: UserDetailsContract;
	userDetailsStore: UserDetailsStore;
}

const UserDetailsLayout = observer(
	({ user, userDetailsStore }: UserDetailsLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes', 'ViewRes.User']);

		const title = user.name;

		useVocaDbTitle(title, true);

		const ownProfile =
			loginManager.loggedUser &&
			loginManager.loggedUser.id === user.id &&
			loginManager.loggedUser.active;

		return (
			<Layout
				title={title}
				subtitle={t(`Resources:UserGroupNames.${user.groupId}`)}
				parents={
					<>
						<Breadcrumb.Item linkAs={Link} linkProps={{ to: '/User' }}>
							{t('ViewRes:Shared.Users')}
						</Breadcrumb.Item>
					</>
				}
				toolbar={
					<>
						{ownProfile && (
							<>
								<JQueryUIButton
									as={Link}
									to="/User/MySettings"
									icons={{ primary: 'ui-icon-wrench' }}
								>
									{t('ViewRes.User:Details.MySettings')}
								</JQueryUIButton>{' '}
								&nbsp;{' '}
								<JQueryUIButton
									as={Link}
									to="/User/Messages"
									icons={{ primary: 'ui-icon-mail-closed' }}
								>
									{t('ViewRes.User:Details.Messages')}
								</JQueryUIButton>
							</>
						)}

						{loginManager.loggedUser &&
							loginManager.loggedUser.id !== user.id &&
							loginManager.loggedUser.active &&
							!user.standalone && (
								<>
									{' '}
									<JQueryUIButton
										as={Link}
										to={`/User/Messages?${qs.stringify({
											receiverName: user.name,
										})}`}
										icons={{ primary: 'ui-icon-mail-closed' }}
									>
										{t('ViewRes.User:Details.ComposeMessage')}
									</JQueryUIButton>
								</>
							)}

						{loginManager.canManageUserPermissions &&
							loginManager.canEditUser(user.groupId) && (
								<>
									{' '}
									&nbsp;{' '}
									<JQueryUIButton
										as="a"
										href={`/User/Edit/${user.id}`}
										icons={{ primary: 'ui-icon-wrench' }}
									>
										{t('ViewRes:Shared.Edit')}
									</JQueryUIButton>
								</>
							)}

						{loginManager.canReportUser &&
							loginManager.loggedUserId !== user.id &&
							user.active && (
								<>
									{' '}
									&nbsp;{' '}
									<JQueryUIButton
										as={SafeAnchor}
										href="#"
										onClick={userDetailsStore.reportUserStore.show}
										icons={{ primary: 'ui-icon-alert' }}
									>
										{t('ViewRes.User:Details.ReportSpamming')}
									</JQueryUIButton>
								</>
							)}

						{loginManager.canRemoveEditPermission &&
							loginManager.loggedUserId !== user.id &&
							user.groupId !== UserGroup.Limited &&
							user.active &&
							loginManager.canEditUser(user.groupId) && (
								<>
									{' '}
									&nbsp;{' '}
									<JQueryUIButton
										as={SafeAnchor}
										href="#"
										onClick={userDetailsStore.limitedUserStore.show}
										icons={{ primary: 'ui-icon-close' }}
									>
										{t('ViewRes:Shared.SetToLimited')}
									</JQueryUIButton>
								</>
							)}

						{loginManager.canDisableUsers &&
							loginManager.loggedUserId !== user.id &&
							user.active && (
								<>
									{' '}
									&nbsp;{' '}
									<JQueryUIButton
										as="a"
										href={`/User/Disable/${user.id}`}
										onClick={(e): void => {
											if (
												!window.confirm(
													t('ViewRes.User:Details.ConfirmDisable'),
												)
											) {
												e.preventDefault();
											}
										}}
										icons={{ primary: 'ui-icon-close' }}
									>
										{t('ViewRes.User:Details.Disable')}
									</JQueryUIButton>
								</>
							)}
					</>
				}
			>
				{ownProfile && user.groupId === UserGroup.Limited && (
					<Alert>
						<h4>Why is my user group "Limited user"?{/* TODO: localize */}</h4>
						<p>
							In order to prevent spammers and abusers we use automated tools to
							check users' IP address. If you have a dynamic IP, it's possible
							that someone with the same IP as you was participating in these
							activities and your account was mistakenly reduced, preventing you
							from editing the database.{/* TODO: localize */}
						</p>
						<p>
							If this is the case, please <a href="/Help">contact us</a> and
							we'll take care of it. Thank you.{/* TODO: localize */}
						</p>
					</Alert>
				)}

				{ownProfile && user.possibleProducerAccount && (
					<Alert variant="info">
						<Trans
							i18nKey="ViewRes.User:Details.PossibleProducerMessage"
							components={{
								// eslint-disable-next-line jsx-a11y/anchor-has-content
								a: <Link to="/User/RequestVerification" />,
							}}
						/>
					</Alert>
				)}

				{ownProfile && user.knownLanguages.length === 0 && (
					<Alert variant="info">
						<Trans
							i18nKey="ViewRes.User:Details.KnownLanguagesMessage"
							components={{
								// eslint-disable-next-line jsx-a11y/anchor-has-content
								a: <Link to="/User/MySettings#profile" />,
							}}
						/>
					</Alert>
				)}

				<UserDetailsRoutes user={user} userDetailsStore={userDetailsStore} />

				{loginManager.canManageUserPermissions && (
					<JQueryUIDialog
						title="StopForumSpam check" /* TODO: localize */
						autoOpen={userDetailsStore.sfsCheckDialog.dialogVisible}
						close={(): void =>
							runInAction(() => {
								userDetailsStore.sfsCheckDialog.dialogVisible = false;
							})
						}
					>
						{userDetailsStore.sfsCheckDialog.html && (
							// HACK
							// TODO: Replace this with React
							<div
								dangerouslySetInnerHTML={{
									__html: userDetailsStore.sfsCheckDialog.html,
								}}
							/>
						)}
					</JQueryUIDialog>
				)}

				<EntryDeletePopupBase
					confirmText="Please confirm that you wish to remove user's editing permissions. You may provide additional explanation below (optional)." /* TODO: localize */
					deleteEntryStore={userDetailsStore.limitedUserStore}
					title="Remove editing permissions" /* TODO: localize */
					deleteButtonProps={{
						text: 'Confirm' /* TODO: localize */,
						icons: { primary: 'ui-icon-close' },
					}}
				/>
				<EntryDeletePopupBase
					confirmText="Please confirm that you wish to report user. Please provide additional explanation below." /* TODO: localize */
					deleteEntryStore={userDetailsStore.reportUserStore}
					title="Report user" /* TODO: localize */
					deleteButtonProps={{
						text: 'Confirm' /* TODO: localize */,
						icons: { primary: 'ui-icon-alert' },
					}}
				/>
			</Layout>
		);
	},
);

const UserDetails = (): React.ReactElement => {
	const [model, setModel] = React.useState<
		| { user: UserDetailsContract; userDetailsStore: UserDetailsStore }
		| undefined
	>();

	const { name } = useParams();

	React.useEffect(() => {
		NProgress.start();

		userRepo
			.getDetails({ name: name! })
			.then((user) => {
				const followedArtistsStore = new FollowedArtistsStore(
					vdb.values,
					userRepo,
					tagRepo,
					user.id,
				);

				const albumCollectionStore = new AlbumCollectionStore(
					vdb.values,
					userRepo,
					artistRepo,
					eventRepo,
					tagRepo,
					user.id,
					user.publicAlbumCollection || user.id === loginManager.loggedUserId,
				);

				const ratedSongsStore = new RatedSongsSearchStore(
					vdb.values,
					urlMapper,
					userRepo,
					artistRepo,
					songRepo,
					tagRepo,
					user.id,
					pvPlayersFactory,
					false,
				);

				setModel({
					user: user,
					userDetailsStore: new UserDetailsStore(
						vdb.values,
						loginManager,
						user.id,
						user.lastLoginAddress,
						loginManager.canDeleteComments,
						httpClient,
						urlMapper,
						userRepo,
						adminRepo,
						tagRepo,
						followedArtistsStore,
						albumCollectionStore,
						ratedSongsStore,
						user.latestComments,
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
	}, [name]);

	return model ? (
		<UserDetailsLayout
			user={model.user}
			userDetailsStore={model.userDetailsStore}
		/>
	) : (
		<></>
	);
};

export default UserDetails;
