import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { showErrorMessage } from '@/Components/ui';
import { ImageHelper } from '@/Helpers/ImageHelper';
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

		const [imagePreview, setImagePreview] = React.useState<string | undefined>(
			undefined,
		);

		const fileInputRef = React.useRef<HTMLInputElement>(null);

		React.useEffect(() => {
			return (): void => {
				if (imagePreview) {
					URL.revokeObjectURL(imagePreview);
				}
			};
		}, [imagePreview]);

		const displayUrl =
			imagePreview || entryPictureFileEditStore.thumbUrl || undefined;

		return (
			<tr>
				<td>
					{displayUrl ? (
						<img
							src={displayUrl}
							alt="Preview" /* LOC */
							className="coverPicThumb"
						/>
					) : null}
					<input
						type="file"
						name="pictureUpload"
						ref={fileInputRef}
						onChange={(e): void => {
							const file = e.target.files?.[0];

							if (imagePreview) {
								URL.revokeObjectURL(imagePreview);
							}

							if (file) {
								const fileExtension =
									'.' + file.name.split('.').pop()?.toLowerCase();
								if (!ImageHelper.allowedExtensions.includes(fileExtension)) {
									showErrorMessage(
										`Invalid format. Allowed: ${ImageHelper.allowedExtensions.join(
											', ',
										)}`,
									);
									e.target.value = '';
									return;
								}

								const maxSizeBytes = ImageHelper.maxImageSizeMB * 1024 * 1024;
								if (file.size > maxSizeBytes) {
									showErrorMessage(
										`Image too large. Maximum size: ${ImageHelper.maxImageSizeMB}MB`,
									);
									e.target.value = '';
									return;
								}

								const previewUrl = URL.createObjectURL(file);
								setImagePreview(previewUrl);
							} else {
								setImagePreview(undefined);
							}
						}}
					/>
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
