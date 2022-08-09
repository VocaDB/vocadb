import { ThumbItem } from '@/Components/Shared/Partials/Shared/ThumbItem';
import { AlbumDetailsForApi } from '@/DataContracts/Album/AlbumDetailsForApi';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { ImageSize } from '@/Models/Images/ImageSize';
import { AlbumDetailsTabs } from '@/Pages/Album/AlbumDetailsRoutes';
import { AlbumDetailsStore } from '@/Stores/Album/AlbumDetailsStore';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface AlbumPicturesProps {
	model: AlbumDetailsForApi;
	albumDetailsStore: AlbumDetailsStore;
}

const AlbumPictures = ({
	model,
	albumDetailsStore,
}: AlbumPicturesProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes.Album']);

	return (
		<AlbumDetailsTabs
			model={model}
			albumDetailsStore={albumDetailsStore}
			tab="pictures"
		>
			<ul className="thumbs">
				<ThumbItem
					href={`/Album/CoverPicture/${model.id}?${qs.stringify({
						v: model.version,
					})}`}
					thumbUrl={UrlHelper.imageThumb(model.mainPicture, ImageSize.Thumb)}
					caption={t('ViewRes.Album:Details.CoverPicture')}
				/>
				{model.pictures.map((picture, index) => (
					<React.Fragment key={index}>
						<ThumbItem
							href={UrlHelper.imageThumb(picture, ImageSize.Original)}
							thumbUrl={UrlHelper.imageThumb(picture, ImageSize.Thumb)}
							caption={picture.name}
						/>
					</React.Fragment>
				))}
			</ul>
		</AlbumDetailsTabs>
	);
};

export default AlbumPictures;
