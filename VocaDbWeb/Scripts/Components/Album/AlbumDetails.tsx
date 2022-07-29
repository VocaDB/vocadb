import Breadcrumb from '@Bootstrap/Breadcrumb';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import AlbumDetailsForApi from '@DataContracts/Album/AlbumDetailsForApi';
import JQueryUIButton from '@JQueryUI/JQueryUIButton';
import AlbumReportType, {
	reportTypesWithRequiredNotes,
} from '@Models/Albums/AlbumReportType';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import LoginManager from '@Models/LoginManager';
import AlbumRepository from '@Repositories/AlbumRepository';
import ArtistRepository from '@Repositories/ArtistRepository';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import AlbumDetailsStore from '@Stores/Album/AlbumDetailsStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

import Layout from '../Shared/Layout';
import DeletedBanner from '../Shared/Partials/EntryDetails/DeletedBanner';
import ReportEntryPopupKnockout from '../Shared/Partials/EntryDetails/ReportEntryPopupKnockout';
import DraftMessage from '../Shared/Partials/Shared/DraftMessage';
import EntryStatusMessage from '../Shared/Partials/Shared/EntryStatusMessage';
import TagsEdit from '../Shared/Partials/TagsEdit';
import useVocaDbTitle from '../useVocaDbTitle';
import AlbumDetailsRoutes from './AlbumDetailsRoutes';
import DownloadTagsDialog from './Partials/DownloadTagsDialog';
import EditCollectionDialog from './Partials/EditCollectionDialog';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const albumRepo = new AlbumRepository(httpClient, vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);

interface AlbumDetailsLayoutProps {
	model: AlbumDetailsForApi;
	albumDetailsStore: AlbumDetailsStore;
}

const AlbumDetailsLayout = observer(
	({
		model,
		albumDetailsStore,
	}: AlbumDetailsLayoutProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Album',
			'VocaDb.Model.Resources.Albums',
		]);

		const titleAndArtist = model.artistString
			? `${model.name} - ${model.artistString}`
			: model.name;

		useVocaDbTitle(titleAndArtist, true);

		return (
			<Layout
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
									entryType: EntryType[EntryType.Album],
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
						<EntryStatusMessage
							status={EntryStatus[model.status as keyof typeof EntryStatus]}
						/>
					</>
				}
			>
				{model.deleted && (
					<DeletedBanner
						mergedTo={
							model.mergedTo
								? { ...model.mergedTo, entryType: EntryType[EntryType.Album] }
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
						notesRequired: reportTypesWithRequiredNotes.includes(r),
					}))}
				/>
			</Layout>
		);
	},
);

const AlbumDetails = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<
		| { model: AlbumDetailsForApi; albumDetailsStore: AlbumDetailsStore }
		| undefined
	>(undefined);

	React.useEffect(() => {
		albumRepo
			.getDetails({ id: Number(id) })
			.then((album) => {
				const model = new AlbumDetailsForApi(album);

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
		<AlbumDetailsLayout
			model={model.model}
			albumDetailsStore={model.albumDetailsStore}
		/>
	) : (
		<></>
	);
};

export default AlbumDetails;
