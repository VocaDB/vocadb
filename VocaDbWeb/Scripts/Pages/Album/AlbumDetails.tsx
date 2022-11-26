import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { DeletedBanner } from '@/Components/Shared/Partials/EntryDetails/DeletedBanner';
import { ReportEntryPopupKnockout } from '@/Components/Shared/Partials/EntryDetails/ReportEntryPopupKnockout';
import { EmbedPVPreview } from '@/Components/Shared/Partials/PV/EmbedPVPreview';
import { DraftMessage } from '@/Components/Shared/Partials/Shared/DraftMessage';
import { EntryStatusMessage } from '@/Components/Shared/Partials/Shared/EntryStatusMessage';
import { TagsEdit } from '@/Components/Shared/Partials/TagsEdit';
import { AlbumDetailsForApi } from '@/DataContracts/Album/AlbumDetailsForApi';
import { PVHelper } from '@/Helpers/PVHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { useLoginManager } from '@/LoginManagerContext';
import {
	AlbumReportType,
	albumReportTypesWithRequiredNotes,
} from '@/Models/Albums/AlbumReportType';
import { EntryType } from '@/Models/EntryType';
import AlbumDetailsRoutes from '@/Pages/Album/AlbumDetailsRoutes';
import DownloadTagsDialog from '@/Pages/Album/Partials/DownloadTagsDialog';
import EditCollectionDialog from '@/Pages/Album/Partials/EditCollectionDialog';
import { albumRepo } from '@/Repositories/AlbumRepository';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { AlbumDetailsStore } from '@/Stores/Album/AlbumDetailsStore';
import { useVdb } from '@/VdbContext';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import NProgress from 'nprogress';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

interface AlbumDetailsLayoutProps {
	model: AlbumDetailsForApi;
	albumDetailsStore: AlbumDetailsStore;
}

const AlbumDetailsLayout = observer(
	({
		model,
		albumDetailsStore,
	}: AlbumDetailsLayoutProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Album',
			'VocaDb.Model.Resources.Albums',
		]);

		const titleAndArtist = model.artistString
			? `${model.name} - ${model.artistString}`
			: model.name;

		return (
			<Layout
				pageTitle={titleAndArtist}
				ready={true}
				title={model.name}
				subtitle={`${model.artistString} (${t(
					`VocaDb.Model.Resources.Albums:DiscTypeNames.${model.discType}`,
				)})`}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: '/Album',
							}}
						>
							{t('ViewRes:Shared.Albums')}
						</Breadcrumb.Item>
					</>
				}
				// TODO: canonicalUrl
				// TODO: robots
				// TODO: head
				toolbar={
					<>
						{model.primaryPV && (
							<div className="song-pv-player">
								<EmbedPVPreview
									entry={{
										...model.contract,
										entryType: EntryType.Album,
									}}
									pv={model.primaryPV}
									allowInline
								/>
							</div>
						)}
						{albumDetailsStore.userHasAlbum ? (
							<JQueryUIButton
								as={SafeAnchor}
								href="#"
								disabled={!loginManager.isLoggedIn}
								onClick={(): void =>
									runInAction(() => {
										albumDetailsStore.editCollectionDialog.dialogVisible = true;
									})
								}
								icons={{ primary: 'ui-icon-wrench' }}
							>
								{t('ViewRes.Album:Details.UpdateCollection')}
							</JQueryUIButton>
						) : (
							<JQueryUIButton
								as={SafeAnchor}
								href="#"
								disabled={!loginManager.isLoggedIn}
								onClick={(): void =>
									runInAction(() => {
										albumDetailsStore.editCollectionDialog.dialogVisible = true;
									})
								}
								icons={{ primary: 'ui-icon-star' }}
							>
								{t('ViewRes.Album:Details.AddToCollection')}
							</JQueryUIButton>
						)}{' '}
						<JQueryUIButton
							as={Link}
							to={`/Album/Edit/${model.id}`}
							disabled={
								!loginManager.canEdit({
									...model.contract,
									entryType: EntryType.Album,
								})
							}
							icons={{ primary: 'ui-icon-wrench' }}
						>
							{t('ViewRes:Shared.Edit')}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as={Link}
							to={`/Album/Versions/${model.id}`}
							icons={{ primary: 'ui-icon-clock' }}
						>
							{t('ViewRes:EntryDetails.ViewModifications')}
						</JQueryUIButton>{' '}
						<span className="ui-buttonset">
							<JQueryUIButton
								as="a"
								href={`/Album/DownloadTags/${model.id}`}
								icons={{ primary: 'ui-icon-arrowthickstop-1-s' }}
							>
								{t('ViewRes.Album:Details.DownloadTrackList')}
							</JQueryUIButton>{' '}
							<JQueryUIButton
								as={SafeAnchor}
								href="#"
								onClick={albumDetailsStore.downloadTagsDialog.show}
								text={false}
								icons={{ primary: 'ui-icon-triangle-1-s' }}
							>
								{t('ViewRes.Album:Details.Customize')}
							</JQueryUIButton>
						</span>{' '}
						<JQueryUIButton
							as={SafeAnchor}
							href="#"
							onClick={albumDetailsStore.reportStore.show}
							icons={{ primary: 'ui-icon-alert' }}
						>
							{t('ViewRes:EntryDetails.ReportAnError')}
						</JQueryUIButton>{' '}
						<EntryStatusMessage status={model.status} />
					</>
				}
			>
				{model.deleted && (
					<DeletedBanner
						mergedTo={
							model.mergedTo
								? { ...model.mergedTo, entryType: EntryType.Album }
								: undefined
						}
					/>
				)}

				{model.draft && !model.deleted && <DraftMessage section="glalbums" />}

				<AlbumDetailsRoutes
					model={model}
					albumDetailsStore={albumDetailsStore}
				/>

				<TagsEdit tagsEditStore={albumDetailsStore.tagsEditStore} />

				<DownloadTagsDialog albumDetailsStore={albumDetailsStore} />

				<EditCollectionDialog albumDetailsStore={albumDetailsStore} />

				<ReportEntryPopupKnockout
					reportEntryStore={albumDetailsStore.reportStore}
					reportTypes={Object.values(AlbumReportType).map((r) => ({
						id: r,
						name: t(`VocaDb.Web.Resources.Domain:EntryReportTypeNames.${r}`),
						notesRequired: albumReportTypesWithRequiredNotes.includes(r),
					}))}
				/>
			</Layout>
		);
	},
);

const AlbumDetails = (): React.ReactElement => {
	const vdb = useVdb();
	const loginManager = useLoginManager();

	const { id } = useParams();

	const [model, setModel] = React.useState<
		| { model: AlbumDetailsForApi; albumDetailsStore: AlbumDetailsStore }
		| undefined
	>(undefined);

	React.useEffect(() => {
		NProgress.start();

		albumRepo
			.getDetails({ id: Number(id) })
			.then((album) => {
				const model = new AlbumDetailsForApi(
					album,
					PVHelper.primaryPV(album.pvs, vdb.values.loggedUser),
				);

				setModel({
					model: model,
					albumDetailsStore: new AlbumDetailsStore(
						vdb.values,
						loginManager,
						albumRepo,
						userRepo,
						artistRepo,
						model.jsonModel,
						loginManager.canDeleteComments,
						loginManager.loggedUser?.albumFormatString ?? '',
						!!model.description.english,
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
	}, [vdb, loginManager, id]);

	return model ? (
		<AlbumDetailsLayout
			model={model.model}
			albumDetailsStore={model.albumDetailsStore}
		/>
	) : (
		<></>
	);
};

export default AlbumDetails;
