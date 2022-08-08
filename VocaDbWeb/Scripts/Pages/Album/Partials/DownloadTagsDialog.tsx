import Button from '@Bootstrap/Button';
import JQueryUIDialog from '@JQueryUI/JQueryUIDialog';
import AlbumDetailsStore from '@Stores/Album/AlbumDetailsStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface DownloadTagsDialogProps {
	albumDetailsStore: AlbumDetailsStore;
}

const DownloadTagsDialog = observer(
	({ albumDetailsStore }: DownloadTagsDialogProps): React.ReactElement => {
		const { t } = useTranslation(['AjaxRes', 'ViewRes.Album']);

		return (
			<JQueryUIDialog
				title={t('ViewRes.Album:Details.DownloadTrackList')}
				autoOpen={albumDetailsStore.downloadTagsDialog.dialogVisible}
				width={400}
				buttons={[
					{
						text: t('AjaxRes:Album.Download'),
						click: (): void => {
							runInAction(() => {
								albumDetailsStore.downloadTagsDialog.dialogVisible = false;
							});

							const url = `/Album/DownloadTags/${albumDetailsStore.downloadTagsDialog.albumId}`;
							window.location.href = `${url}?setFormatString=true&formatString=${encodeURIComponent(
								albumDetailsStore.downloadTagsDialog.formatString,
							)}`;
						},
					},
				]}
				close={(): void =>
					runInAction(() => {
						albumDetailsStore.downloadTagsDialog.dialogVisible = false;
					})
				}
			>
				<div>
					<label>{t('ViewRes.Album:Details.CustomFormatStringLabel')}</label>
					<div className="form-inline">
						<input
							type="text"
							value={albumDetailsStore.downloadTagsDialog.formatString}
							onChange={(e): void =>
								runInAction(() => {
									albumDetailsStore.downloadTagsDialog.formatString =
										e.target.value;
								})
							}
							maxLength={200}
							className="input-xlarge"
						/>{' '}
						<Button
							onClick={(): void =>
								runInAction(() => {
									albumDetailsStore.downloadTagsDialog.formatString = '';
								})
							}
						>
							{t('ViewRes.Album:Details.Reset')}
						</Button>
					</div>
				</div>
			</JQueryUIDialog>
		);
	},
);

export default DownloadTagsDialog;
