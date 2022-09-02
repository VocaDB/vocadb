import { ThumbItem } from '@/Components/Shared/Partials/Shared/ThumbItem';
import { ArtistDetailsContract } from '@/DataContracts/Artist/ArtistDetailsContract';
import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { ImageSize } from '@/Models/Images/ImageSize';
import { ArtistDetailsTabs } from '@/Pages/Artist/ArtistDetailsRoutes';
import { ArtistDetailsStore } from '@/Stores/Artist/ArtistDetailsStore';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ArtistPictureThumbItemProps {
	picture: EntryThumbContract;
}

const ArtistPictureThumbItem = React.memo(
	({ picture }: ArtistPictureThumbItemProps): React.ReactElement => {
		return (
			<ThumbItem
				href={UrlHelper.imageThumb(picture, ImageSize.Original)}
				thumbUrl={UrlHelper.imageThumb(picture, ImageSize.Thumb)}
				caption={picture.name}
			/>
		);
	},
);

interface ArtistPicturesProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistPictures = React.memo(
	({ artist, artistDetailsStore }: ArtistPicturesProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Album']);

		return (
			<ArtistDetailsTabs
				artist={artist}
				artistDetailsStore={artistDetailsStore}
				tab="pictures"
			>
				<div className="thumbs">
					<ThumbItem
						href={`/Artist/Picture/${artist.id}`}
						thumbUrl={`/Artist/PictureThumb/${artist.id}`}
						caption={t('ViewRes.Album:Details.CoverPicture')}
					/>
					{artist.pictures.map((picture, index) => (
						<React.Fragment key={index}>
							<ArtistPictureThumbItem picture={picture} />
						</React.Fragment>
					))}
				</div>
			</ArtistDetailsTabs>
		);
	},
);

export default ArtistPictures;
