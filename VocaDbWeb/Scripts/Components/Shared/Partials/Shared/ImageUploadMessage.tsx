import ImageHelper from '@Helpers/ImageHelper';
import React from 'react';
import { useTranslation } from 'react-i18next';

const ImageUploadMessage = React.memo(
	(): React.ReactElement => {
		const { t } = useTranslation(['HelperRes']);

		return (
			<p>
				{t('HelperRes:Helper.UploadPictureInfo', {
					0: ImageHelper.allowedExtensions.join(', '),
					1: ImageHelper.maxImageSizeMB,
				})}
			</p>
		);
	},
);

export default ImageUploadMessage;
