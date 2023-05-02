import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { EntryDeletePopupBase } from '@/Components/Shared/Partials/EntryDetails/EntryDeletePopupBase';
import { UserDetailsContract } from '@/DataContracts/User/UserDetailsContract';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import JQueryUIDialog from '@/JQueryUI/JQueryUIDialog';
import { useLoginManager } from '@/LoginManagerContext';
import { UserGroup } from '@/Models/Users/UserGroup';
import { useMutedUsers } from '@/MutedUsersContext';
import UserDetailsRoutes from '@/Pages/User/UserDetailsRoutes';
import { adminRepo } from '@/Repositories/AdminRepository';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { tagRepo } from '@/Repositories/TagRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { urlMapper } from '@/Shared/UrlMapper';
import { AlbumCollectionStore } from '@/Stores/User/AlbumCollectionStore';
import { FollowedArtistsStore } from '@/Stores/User/FollowedArtistsStore';
import { RatedSongsSearchStore } from '@/Stores/User/RatedSongsSearchStore';
import { UserDetailsStore } from '@/Stores/User/UserDetailsStore';
import { useVdb } from '@/VdbContext';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import NProgress from 'nprogress';
import qs from 'qs';
import React from 'react';
import { Trans, useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

import '@/styles/Styles/songlist.less'

interface UserDetailsLayoutProps {
	user: UserDetailsContract;
	userDetailsStore: UserDetailsStore;
}

const UserDetailsLayout = observer(
	({ user, userDetailsStore }: UserDetailsLayoutProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t } = useTranslation(['Resources', 'ViewRes', 'ViewRes.User']);

		const title = user.name;

		const ownProfile =
			loginManager.loggedUser &&
			loginManager.loggedUser.id === user.id &&
			loginManager.loggedUser.active;

		const mutedUsers = useMutedUsers();

		const mutedUser = mutedUsers.find(user.id);

		return (
			<Layout
				pageTitle={title}
				ready={true}
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
						{ownProfile ? (
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
						) : (
							<>
								{mutedUser ? (
									<JQueryUIButton
										as={SafeAnchor}
										onClick={(): void => mutedUsers.removeMutedUser(mutedUser)}
										icons={{ primary: 'ui-icon-volume-on' }}
									>
										Unmute{/* LOC */}
									</JQueryUIButton>
								) : (
									<JQueryUIButton
										as={SafeAnchor}
										onClick={(): void => mutedUsers.addMutedUser(user.id)}
										icons={{ primary: 'ui-icon-volume-off' }}
									>
										Mute{/* LOC */}
									</JQueryUIButton>
								)}
							</>
						)}

						{loginManager.loggedUser &&
							loginManager.loggedUser.id !== user.id &&
							loginManager.loggedUser.active &&
							!user.standalone && (
								<>
									{' '}
									&nbsp;{' '}
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
										as={Link}
										to={`/User/Edit/${user.id}`}
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
						<h4>Why is my user group "Limited user"?{/* LOC */}</h4>
						<p>
							In order to prevent spammers and abusers we use automated tools to
							check users' IP address. If you have a dynamic IP, it's possible
							that someone with the same IP as you was participating in these
							activities and your account was mistakenly reduced, preventing you
							from editing the database.{/* LOC */}
						</p>
						<p>
							If this is the case, please <a href="/Help">contact us</a> and
							we'll take care of it. Thank you.{/* LOC */}
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
						title="StopForumSpam check" /* LOC */
						autoOpen={userDetailsStore.sfsCheckDialog.dialogVisible}
						width={310}
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
					confirmText="Please confirm that you wish to remove user's editing permissions. You may provide additional explanation below (optional)." /* LOC */
					deleteEntryStore={userDetailsStore.limitedUserStore}
					title="Remove editing permissions" /* LOC */
					deleteButtonProps={{
						text: 'Confirm' /* LOC */,
						icons: { primary: 'ui-icon-close' },
					}}
					onDelete={(): void => {
						// TODO
						window.location.reload();
					}}
				/>
				<EntryDeletePopupBase
					confirmText="Please confirm that you wish to report user. Please provide additional explanation below." /* LOC */
					deleteEntryStore={userDetailsStore.reportUserStore}
					title="Report user" /* LOC */
					deleteButtonProps={{
						text: 'Confirm' /* LOC */,
						icons: { primary: 'ui-icon-alert' },
					}}
					onDelete={(): void => {
						// TODO: showSuccessMessage
						runInAction(() => {
							userDetailsStore.reportUserStore.notes = '';
						});
					}}
				/>
			</Layout>
		);
	},
);

const UserDetails = (): React.ReactElement => {
	const vdb = useVdb();
	const loginManager = useLoginManager();

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
						antiforgeryRepo,
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
	}, [vdb, loginManager, name]);

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
