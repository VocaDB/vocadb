import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { DeletedBanner } from '@/Components/Shared/Partials/EntryDetails/DeletedBanner';
import { ReportEntryPopupKnockout } from '@/Components/Shared/Partials/EntryDetails/ReportEntryPopupKnockout';
import { AlbumSearchDropdown } from '@/Components/Shared/Partials/Knockout/SearchDropdown';
import { DraftMessage } from '@/Components/Shared/Partials/Shared/DraftMessage';
import { EntryStatusMessage } from '@/Components/Shared/Partials/Shared/EntryStatusMessage';
import { ArtistDetailsContract } from '@/DataContracts/Artist/ArtistDetailsContract';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import {
	ArtistReportType,
	artistReportTypesWithRequiredNotes,
} from '@/Models/Artists/ArtistReportType';
import { EntryType } from '@/Models/EntryType';
import { loginManager } from '@/Models/LoginManager';
import ArtistDetailsRoutes from '@/Pages/Artist/ArtistDetailsRoutes';
import CustomizeArtistSubscriptionDialog from '@/Pages/Artist/Partials/CustomizeArtistSubscriptionDialog';
import { albumRepo } from '@/Repositories/AlbumRepository';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { urlMapper } from '@/Shared/UrlMapper';
import { ArtistDetailsStore } from '@/Stores/Artist/ArtistDetailsStore';
import { PVPlayersFactory } from '@/Stores/PVs/PVPlayersFactory';
import { AlbumSearchStore } from '@/Stores/Search/AlbumSearchStore';
import classNames from 'classnames';
import { reaction, runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import NProgress from 'nprogress';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

import '../../../wwwroot/Content/Styles/songlist.less';

const pvPlayersFactory = new PVPlayersFactory();

interface AlbumOptionsProps {
	albumSearchStore: AlbumSearchStore;
}

export const AlbumOptions = observer(
	({ albumSearchStore }: AlbumOptionsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Artist']);

		return (
			<div className="clearfix">
				<div className="pull-right">
					<AlbumSearchDropdown albumSearchStore={albumSearchStore} />{' '}
					<ButtonGroup>
						<Button
							onClick={(): void =>
								runInAction(() => {
									albumSearchStore.viewMode = 'Details';
								})
							}
							className={classNames(
								'btn-nomargin',
								albumSearchStore.viewMode === 'Details' && 'active',
							)}
							href="#"
							title={t('ViewRes.Artist:Details.ViewModeDetails')}
						>
							<i className="icon-list noMargin" />{' '}
							{t('ViewRes.Artist:Details.ViewModeDetails')}
						</Button>
						<Button
							onClick={(): void =>
								runInAction(() => {
									albumSearchStore.viewMode = 'Tiles';
								})
							}
							className={classNames(
								'btn-nomargin',
								albumSearchStore.viewMode === 'Tiles' && 'active',
							)}
							href="#"
							title={t('ViewRes.Artist:Details.ViewModeTiles')}
						>
							<i className="icon-th noMargin" />{' '}
							{t('ViewRes.Artist:Details.ViewModeTiles')}
						</Button>
					</ButtonGroup>
				</div>
			</div>
		);
	},
);

interface ArtistDetailsLayoutProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistDetailsLayout = observer(
	({
		artist,
		artistDetailsStore,
	}: ArtistDetailsLayoutProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Artist',
			'VocaDb.Model.Resources',
			'VocaDb.Web.Resources.Domain',
		]);

		const title = artist.name;

		React.useEffect(() => {
			// Returns the disposer.
			return reaction(
				() =>
					artistDetailsStore.customizeSubscriptionDialog.notificationsMethod,
				async (method) => {
					await userRepo.updateArtistSubscription({
						artistId: artistDetailsStore.artistId,
						emailNotifications: method === 'Email',
						siteNotifications: method === 'Site' || method === 'Email',
					});
				},
			);
		}, [artistDetailsStore]);

		return (
			<Layout
				pageTitle={title}
				ready={true}
				title={title}
				subtitle={`(${t(
					`VocaDb.Model.Resources:ArtistTypeNames.${artist.artistType}`,
				)})`}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: '/Artist',
							}}
						>
							{t('ViewRes:Shared.Artists')}
						</Breadcrumb.Item>
					</>
				}
				// TODO: canonicalUrl
				// TODO: robots
				// TODO: head
				toolbar={
					<>
						{artistDetailsStore.hasArtistSubscription ? (
							<span id="removeFromUserSplitBtn" className="ui-buttonset">
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									disabled={!loginManager.isLoggedIn}
									onClick={artistDetailsStore.removeFollowedArtist}
									icons={{ primary: 'ui-icon-close' }}
								>
									{t('ViewRes.Artist:Details:Unfollow')}
								</JQueryUIButton>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									onClick={artistDetailsStore.customizeSubscriptionDialog.show}
									text={false}
									icons={{ primary: 'ui-icon-triangle-1-s' }}
								>
									{t('ViewRes.Artist:Details:Customize')}
								</JQueryUIButton>
							</span>
						) : (
							<JQueryUIButton
								as={SafeAnchor}
								href="#"
								disabled={!loginManager.isLoggedIn}
								onClick={artistDetailsStore.addFollowedArtist}
								icons={{ primary: 'ui-icon-heart' }}
							>
								{t('ViewRes.Artist:Details:Follow')}
							</JQueryUIButton>
						)}{' '}
						<JQueryUIButton
							as={Link}
							to={`/Artist/Edit/${artist.id}`}
							disabled={
								!loginManager.canEdit({
									...artist,
									entryType: EntryType.Artist,
								})
							}
							icons={{ primary: 'ui-icon-wrench' }}
						>
							{t('ViewRes:Shared.Edit')}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as={Link}
							to={`/Artist/Versions/${artist.id}`}
							icons={{ primary: 'ui-icon-clock' }}
						>
							{t('ViewRes:EntryDetails.ViewModifications')}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as={SafeAnchor}
							href="#"
							onClick={artistDetailsStore.reportStore.show}
							icons={{ primary: 'ui-icon-alert' }}
						>
							{t('ViewRes:EntryDetails.ReportAnError')}
						</JQueryUIButton>{' '}
						<EntryStatusMessage status={artist.status} />
					</>
				}
			>
				{artist.deleted && (
					<DeletedBanner
						mergedTo={
							artist.mergedTo
								? { ...artist.mergedTo, entryType: EntryType.Artist }
								: undefined
						}
					/>
				)}

				{artist.draft && !artist.deleted && (
					<DraftMessage section="glproducers" />
				)}

				<ArtistDetailsRoutes
					artist={artist}
					artistDetailsStore={artistDetailsStore}
				/>

				<ReportEntryPopupKnockout
					reportEntryStore={artistDetailsStore.reportStore}
					reportTypes={Object.values(ArtistReportType)
						.filter((r) => r !== ArtistReportType.OwnershipClaim)
						.map((r) => ({
							id: r,
							name: t(`VocaDb.Web.Resources.Domain:EntryReportTypeNames.${r}`),
							notesRequired: artistReportTypesWithRequiredNotes.includes(r),
						}))}
				/>

				{loginManager.isLoggedIn && (
					<CustomizeArtistSubscriptionDialog
						customizeArtistSubscriptionStore={
							artistDetailsStore.customizeSubscriptionDialog
						}
					/>
				)}
			</Layout>
		);
	},
);

const ArtistDetails = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<
		| { artist: ArtistDetailsContract; artistDetailsStore: ArtistDetailsStore }
		| undefined
	>(undefined);

	React.useEffect(() => {
		NProgress.start();

		artistRepo
			.getDetails({ id: Number(id) })
			.then((artist) => {
				setModel({
					artist: artist,
					artistDetailsStore: new ArtistDetailsStore(
						vdb.values,
						loginManager,
						artistRepo,
						artist.id,
						artist.tags,
						artist.isAdded,
						artist.emailNotifications,
						artist.siteNotifications,
						!!artist.description.english,
						urlMapper,
						albumRepo,
						songRepo,
						userRepo,
						loginManager.canDeleteComments,
						pvPlayersFactory,
						artist.latestComments,
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
	}, [id]);

	return model ? (
		<ArtistDetailsLayout
			artist={model.artist}
			artistDetailsStore={model.artistDetailsStore}
		/>
	) : (
		<></>
	);
};

export default ArtistDetails;
