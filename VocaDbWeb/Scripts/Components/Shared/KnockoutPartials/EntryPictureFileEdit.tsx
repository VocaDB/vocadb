import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { EntryPictureFileEditStore } from '@/Stores/EntryPictureFileEditStore';
import { EntryPictureFileListEditStore } from '@/Stores/EntryPictureFileListEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EntryPictureFileEditProps {
	entryPictureFileListEditStore: EntryPictureFileListEditStore;
	entryPictureFileEditStore: EntryPictureFileEditStore;
}

export const EntryPictureFileEdit = observer(
	({
		entryPictureFileListEditStore,
		entryPictureFileEditStore,
	}: EntryPictureFileEditProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Artist']);

		return (
			<tr>
				<td>
					{entryPictureFileEditStore.thumbUrl ? (
						<img
							src={entryPictureFileEditStore.thumbUrl}
							alt="Preview" /* LOCALIZE */
							className="coverPicThumb"
						/>
					) : (
						<input type="file" name="pictureUpload" />
					)}
				</td>
				<td>
					<input
						type="text"
						value={entryPictureFileEditStore.name}
						onChange={(e): void =>
							runInAction(() => {
								entryPictureFileEditStore.name = e.target.value;
							})
						}
						size={40}
						maxLength={200}
						className="input-xlarge"
						placeholder={t('ViewRes.Artist:Edit.PiPictureDescription')}
					/>
				</td>
				<td>
					<SafeAnchor
						onClick={(): void =>
							entryPictureFileListEditStore.remove(entryPictureFileEditStore)
						}
						href="#"
						className="textLink deleteLink"
					>
						{t('ViewRes:Shared.Delete')}
					</SafeAnchor>
				</td>
			</tr>
		);
	},
);
