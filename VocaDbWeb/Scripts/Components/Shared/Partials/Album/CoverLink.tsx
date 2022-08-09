import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { ImageSize } from '@/Models/Images/ImageSize';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface CoverLinkProps {
	imageInfo?: EntryThumbContract;
}

export const CoverLink = React.memo(
	({ imageInfo }: CoverLinkProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Album']);

		return (
			<a href={UrlHelper.imageThumb(imageInfo, ImageSize.Original)}>
				<img
					src={UrlHelper.imageThumb(imageInfo, ImageSize.Thumb)}
					alt={t('ViewRes.Album:Details.CoverPicture')}
					className="coverPic"
				/>
			</a>
		);
	},
);
