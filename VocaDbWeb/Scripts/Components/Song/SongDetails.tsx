import Breadcrumb from '@Bootstrap/Breadcrumb';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import SongDetailsForApi from '@DataContracts/Song/SongDetailsForApi';
import JQueryUIButton from '@JQueryUI/JQueryUIButton';
import JQueryUIDialog from '@JQueryUI/JQueryUIDialog';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import LoginManager from '@Models/LoginManager';
import SongVoteRating from '@Models/SongVoteRating';
import SongReportType, {
	reportTypesWithRequiredNotes,
} from '@Models/Songs/SongReportType';
import ArtistRepository from '@Repositories/ArtistRepository';
import SongRepository from '@Repositories/SongRepository';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import { SearchType } from '@Stores/Search/SearchStore';
import SongDetailsStore from '@Stores/Song/SongDetailsStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams, useSearchParams } from 'react-router-dom';

import Layout from '../Shared/Layout';
import DeletedBanner from '../Shared/Partials/EntryDetails/DeletedBanner';
import ReportEntryPopupKnockout from '../Shared/Partials/EntryDetails/ReportEntryPopupKnockout';
import EmbedPVPreview from '../Shared/Partials/PV/EmbedPVPreview';
import DraftMessage from '../Shared/Partials/Shared/DraftMessage';
import EntryStatusMessage from '../Shared/Partials/Shared/EntryStatusMessage';
import TagsEdit from '../Shared/Partials/TagsEdit';
import useVocaDbTitle from '../useVocaDbTitle';
import AddToListDialog from './Partials/AddToListDialog';
import SongDetailsRoutes from './SongDetailsRoutes';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const songRepo = new SongRepository(httpClient, vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);

interface SongDetailsLayoutProps {
	model: SongDetailsForApi;
	songDetailsStore: SongDetailsStore;
}

const SongDetailsLayout = observer(
	({ model, songDetailsStore }: SongDetailsLayoutProps): React.ReactElement => {
		const { t } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.Song',
			'VocaDb.Model.Resources.Songs',
			'VocaDb.Web.Resources.Domain',
		]);

		const titleAndArtist = `${model.name} - ${model.artistString}`;

		useVocaDbTitle(titleAndArtist, true);

		const primaryPV = model.contract.pvs.filter(
			(pv) => pv.id === songDetailsStore.selectedPvId,
		)[0];

		return (
			<Layout
				title={model.name}
				subtitle={`${model.artistString} (${t(
					`VocaDb.Model.Resources.Songs:SongTypeNames.${model.songType}`,
				)})`}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Search?${qs.stringify({ searchType: SearchType.Song })}`,
							}}
						>
							{t('ViewRes:Shared.Songs')}
						</Breadcrumb.Item>
					</>
				}
				// TODO: Head
				toolbar={
					<>
						{(model.originalPVs.length > 0 || model.otherPVs.length > 0) && (
							<div id="pvPlayer" className="song-pv-player">
								{primaryPV && (
									<EmbedPVPreview
										entry={{
											...model.contract.song,
											entryType: EntryType[EntryType.Song],
										}}
										pv={primaryPV}
									/>
								)}
							</div>
						)}
						{loginManager.isLoggedIn ? (
							<>
								<span>
									{songDetailsStore.userRating.isRatingFavorite && (
										<span
											className="icon heartIcon"
											title={t(
												`Resources:SongVoteRatingNames.${
													SongVoteRating[SongVoteRating.Favorite]
												}`,
											)}
										/>
									)}
									{songDetailsStore.userRating.isRatingLike && (
										<span
											className="icon starIcon"
											title={t(
												`Resources:SongVoteRatingNames.${
													SongVoteRating[SongVoteRating.Like]
												}`,
											)}
										/>
									)}
									{songDetailsStore.userRating.isRated ? (
										<>
											{' '}
											<JQueryUIButton
												as={SafeAnchor}
												href="#"
												onClick={songDetailsStore.userRating.setRating_nothing}
												icons={{ primary: 'ui-icon-close' }}
												disabled={songDetailsStore.userRating.ratingInProgress}
											>
												{t('ViewRes.Song:Details.RemoveFromFavorites')}
											</JQueryUIButton>
										</>
									) : (
										<span className="js-ratingButtons">
											<JQueryUIButton
												as={SafeAnchor}
												href="#"
												onClick={songDetailsStore.userRating.setRating_like}
												icons={{ primary: 'ui-icon-plus' }}
												disabled={songDetailsStore.userRating.ratingInProgress}
											>
												{t('ViewRes.Song:Details.Like')}
											</JQueryUIButton>
											<JQueryUIButton
												as={SafeAnchor}
												href="#"
												onClick={songDetailsStore.userRating.setRating_favorite}
												icons={{ primary: 'ui-icon-heart' }}
												disabled={songDetailsStore.userRating.ratingInProgress}
											>
												{t('ViewRes.Song:Details.AddToFavorites')}
											</JQueryUIButton>
										</span>
									)}
								</span>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									onClick={songDetailsStore.songListDialog.showSongLists}
									icons={{ primary: 'ui-icon-star' }}
									id="addToListLink"
								>
									{t('ViewRes.Song:Details.AddToCustomList')}
								</JQueryUIButton>
							</>
						) : (
							<>
								<span className="js-ratingButtons">
									<JQueryUIButton
										as={SafeAnchor}
										href="#"
										disabled={true}
										icons={{ primary: 'ui-icon-plus' }}
									>
										{t('ViewRes.Song:Details.Like')}
									</JQueryUIButton>
									<JQueryUIButton
										as={SafeAnchor}
										href="#"
										disabled={true}
										icons={{ primary: 'ui-icon-heart' }}
									>
										{t('ViewRes.Song:Details.AddToFavorites')}
									</JQueryUIButton>
								</span>{' '}
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									disabled={true}
									icons={{ primary: 'ui-icon-star' }}
								>
									{t('ViewRes.Song:Details.AddToCustomList')}
								</JQueryUIButton>
							</>
						)}{' '}
						<JQueryUIButton
							as="a"
							href={`/Song/Edit/${model.id}?${qs.stringify({
								albumId: model.browsedAlbumId,
							})}`}
							disabled={
								!loginManager.canEdit({
									...model.contract.song,
									entryType: EntryType[EntryType.Song],
								})
							}
							icons={{ primary: 'ui-icon-wrench' }}
						>
							{t('ViewRes:Shared.Edit')}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as="a"
							href={`/Song/Versions/${model.id}`}
							icons={{ primary: 'ui-icon-clock' }}
						>
							{t('ViewRes:EntryDetails.ViewModifications')}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as={SafeAnchor}
							href="#"
							onClick={songDetailsStore.reportStore.show}
							icons={{ primary: 'ui-icon-alert' }}
						>
							{t('ViewRes:EntryDetails.ReportAnError')}
						</JQueryUIButton>
						{loginManager.canAccessManageMenu && (
							<>
								{' '}
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									onClick={(): void =>
										runInAction(() => {
											songDetailsStore.maintenanceDialogVisible = true;
										})
									}
									icons={{ primary: 'ui-icon-wrench' }}
								>
									Maintenance actions{/* TODO: localize */}
								</JQueryUIButton>
							</>
						)}{' '}
						<EntryStatusMessage
							status={EntryStatus[model.status as keyof typeof EntryStatus]}
						/>
						{/* TODO: _AjaxLoader */}
					</>
				}
			>
				{model.deleted && (
					<DeletedBanner
						mergedTo={
							model.mergedTo
								? { ...model.mergedTo, entryType: EntryType[EntryType.Song] }
								: undefined
						}
					/>
				)}

				{model.draft && !model.deleted && <DraftMessage section="glsongs" />}

				<SongDetailsRoutes model={model} songDetailsStore={songDetailsStore} />

				<TagsEdit tagsEditStore={songDetailsStore.tagsEditStore} />

				<AddToListDialog songListsStore={songDetailsStore.songListDialog} />

				<ReportEntryPopupKnockout
					reportEntryStore={songDetailsStore.reportStore}
					reportTypes={Object.values(SongReportType).map((r) => ({
						id: r,
						name: t(`VocaDb.Web.Resources.Domain:EntryReportTypeNames.${r}`),
						notesRequired: reportTypesWithRequiredNotes.includes(r),
					}))}
				/>

				<JQueryUIDialog
					title="Maintenance actions" /* TODO: localize */
					autoOpen={songDetailsStore.maintenanceDialogVisible}
					width={400}
					close={(): void =>
						runInAction(() => {
							songDetailsStore.maintenanceDialogVisible = false;
						})
					}
				>
					<div>
						<p>
							<JQueryUIButton
								as="a"
								href={`/Song/UpdateArtistString/${model.id}`}
							>
								Refresh artist string{/* TODO: localize */}
							</JQueryUIButton>
						</p>
						<p>
							<JQueryUIButton as="a" href={`/Song/UpdateThumbUrl/${model.id}`}>
								Refresh thumbnail{/* TODO: localize */}
							</JQueryUIButton>
						</p>
						<p>
							<JQueryUIButton
								as="a"
								href={`/Song/RefreshPVMetadatas/${model.id}`}
							>
								Refresh PV metadata{/* TODO: localize */}
							</JQueryUIButton>
						</p>
					</div>
				</JQueryUIDialog>
			</Layout>
		);
	},
);

const SongDetails = (): React.ReactElement => {
	const [model, setModel] = React.useState<
		{ model: SongDetailsForApi; songDetailsStore: SongDetailsStore } | undefined
	>();

	const { id } = useParams();
	const [searchParams] = useSearchParams();
	const albumId = searchParams.get('albumId');

	React.useEffect(() => {
		songRepo
			.getDetails({
				id: Number(id),
				albumId: albumId ? Number(albumId) : undefined,
			})
			.then((song) => {
				const model = new SongDetailsForApi(song);

				setModel({
					model: model,
					songDetailsStore: new SongDetailsStore(
						vdb.values,
						loginManager,
						httpClient,
						songRepo,
						userRepo,
						artistRepo,
						(vdb.values.languagePreference ===
							ContentLanguagePreference.English ||
							vdb.values.languagePreference ===
								ContentLanguagePreference.Romaji) &&
							!!model.notes.english,
						model.jsonModel,
						loginManager.canDeleteComments,
					),
				});
			})
			.catch((error) => {
				if (error.response) {
					if (error.response.status === 404)
						window.location.href = '/Error/NotFound';
				}

				throw error;
			});
	}, [id, albumId]);

	return model ? (
		<SongDetailsLayout
			model={model.model}
			songDetailsStore={model.songDetailsStore}
		/>
	) : (
		<></>
	);
};

export default SongDetails;
