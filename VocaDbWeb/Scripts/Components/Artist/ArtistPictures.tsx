import ThumbItem from '@Components/Shared/Partials/Shared/ThumbItem';
import ArtistDetailsContract from '@DataContracts/Artist/ArtistDetailsContract';
import UrlHelper from '@Helpers/UrlHelper';
import ImageSize from '@Models/Images/ImageSize';
import ArtistDetailsStore from '@Stores/Artist/ArtistDetailsStore';
import React from 'react';
import { useTranslation } from 'react-i18next';

import { ArtistDetailsTabs } from './ArtistDetailsRoutes';

interface ArtistPicturesProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistPictures = ({
	artist,
	artistDetailsStore,
}: ArtistPicturesProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes.Album']);

	return (
		<ArtistDetailsTabs
			artist={artist}
			artistDetailsStore={artistDetailsStore}
			tab="pictures"
		>
			<ul className="thumbs">
				<ThumbItem
					href={`/Artist/Picture/${artist.id}`}
					thumbUrl={`/Artist/PictureThumb/${artist.id}`}
					caption={t('ViewRes.Album:Details.CoverPicture')}
				/>
				{artist.pictures.map((picture, index) => (
					<React.Fragment key={index}>
						<ThumbItem
							href={UrlHelper.imageThumb(picture, ImageSize.Original)}
							thumbUrl={UrlHelper.imageThumb(picture, ImageSize.Thumb)}
							caption={picture.name}
						/>
					</React.Fragment>
				))}
			</ul>
		</ArtistDetailsTabs>
	);
};

export default ArtistPictures;
