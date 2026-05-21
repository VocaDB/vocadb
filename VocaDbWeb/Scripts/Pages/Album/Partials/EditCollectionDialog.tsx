import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { showSuccessMessage } from '@/Components/ui';
import {
	MediaType,
	PurchaseStatus,
} from '@/DataContracts/User/AlbumForUserForApiContract';
import JQueryUIDialog from '@/JQueryUI/JQueryUIDialog';
import { urlMapper } from '@/Shared/UrlMapper';
import { AlbumDetailsStore } from '@/Stores/Album/AlbumDetailsStore';
import { RatingPicker } from '@/Components/Shared/Partials/Shared/RatingPicker';
import $ from 'jquery';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EditCollectionDialogProps {
	albumDetailsStore: AlbumDetailsStore;
}

const EditCollectionDialog = observer(
	({ albumDetailsStore }: EditCollectionDialogProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes', 'ViewRes.Album']);

		return (
			<JQueryUIDialog
				title={t('ViewRes.Album:Details.EditAlbumInCollection')}
				autoOpen={albumDetailsStore.editCollectionDialog.dialogVisible}
				width={320}
				buttons={[
					{
						text: t('ViewRes:Shared.Save'),
						click: async (): Promise<void> => {
							runInAction(() => {
								albumDetailsStore.editCollectionDialog.dialogVisible = false;
							});

							await $.post(urlMapper.mapRelative('/User/UpdateAlbumForUser'), {
								albumId: albumDetailsStore.editCollectionDialog.albumId,
								collectionStatus:
									albumDetailsStore.editCollectionDialog.albumPurchaseStatus,
								mediaType:
									albumDetailsStore.editCollectionDialog.albumMediaType,
								rating: albumDetailsStore.editCollectionDialog.collectionRating,
							});

							runInAction(() => {
								albumDetailsStore.userHasAlbum =
									albumDetailsStore.editCollectionDialog.albumPurchaseStatus !==
									PurchaseStatus.Nothing;
							});

							showSuccessMessage(t('AjaxRes:Album.AddedToCollection'));
						},
					},
				]}
				close={(): void =>
					runInAction(() => {
						albumDetailsStore.editCollectionDialog.dialogVisible = false;
					})
				}
			>
				{t('ViewRes.Album:Details.Status')}:{' '}
				<select
					className="input-medium"
					value={albumDetailsStore.editCollectionDialog.albumPurchaseStatus}
					onChange={(e): void =>
						runInAction(() => {
							albumDetailsStore.editCollectionDialog.albumPurchaseStatus = e
								.target.value as PurchaseStatus;
						})
					}
				>
					{Object.values(PurchaseStatus).map((value) => (
						<option value={value} key={value}>
							{t(`Resources:AlbumCollectionStatusNames.${value}`)}
						</option>
					))}
				</select>
				<br />
				{t('ViewRes.Album:Details.MediaType')}:{' '}
				<select
					className="input-medium"
					value={albumDetailsStore.editCollectionDialog.albumMediaType}
					onChange={(e): void =>
						runInAction(() => {
							albumDetailsStore.editCollectionDialog.albumMediaType = e.target
								.value as MediaType;
						})
					}
				>
					{[
						MediaType.Other,
						MediaType.PhysicalDisc,
						MediaType.DigitalDownload,
					].map((value) => (
						<option value={value} key={value}>
							{t(`Resources:AlbumMediaTypeNames.${value}`)}
						</option>
					))}
				</select>
				<div>
					{t('ViewRes.Album:Details.MyRating')}
					<br />
					<RatingPicker
						value={albumDetailsStore.editCollectionDialog.collectionRating}
						onChange={(value): void => {
							runInAction(() => {
								albumDetailsStore.editCollectionDialog.collectionRating = value;
							});
						}}
					/>
					&nbsp;{' '}
					<SafeAnchor
						href="#"
						onClick={(): void =>
							runInAction(() => {
								albumDetailsStore.editCollectionDialog.collectionRating = 0;
							})
						}
						className="textLink deleteLink"
					>
						{t('ViewRes:Shared.Delete')}
					</SafeAnchor>
				</div>
			</JQueryUIDialog>
		);
	},
);

export default EditCollectionDialog;
