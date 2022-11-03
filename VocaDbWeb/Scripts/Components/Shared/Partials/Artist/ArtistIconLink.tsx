import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { EntryType } from '@/Models/EntryType';
import { ImageSize } from '@/Models/Images/ImageSize';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface ArtistIconLinkProps {
	artist: ArtistApiContract;
}

export const ArtistIconLink = ({
	artist,
}: ArtistIconLinkProps): React.ReactElement => {
	return (
		<Link to={EntryUrlMapper.details(EntryType.Artist, artist.id)}>
			<img
				src={UrlHelper.imageThumb(artist.mainPicture, ImageSize.TinyThumb)}
				alt="Thumb" /* LOCALIZE */
				className="coverPicThumb"
			/>
		</Link>
	);
};
