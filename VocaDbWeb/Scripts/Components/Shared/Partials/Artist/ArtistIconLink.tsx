import ArtistApiContract from '@DataContracts/Artist/ArtistApiContract';
import UrlHelper from '@Helpers/UrlHelper';
import EntryType from '@Models/EntryType';
import ImageSize from '@Models/Images/ImageSize';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

interface ArtistIconLinkProps {
	artist: ArtistApiContract;
}

const ArtistIconLink = ({
	artist,
}: ArtistIconLinkProps): React.ReactElement => {
	return (
		<a href={EntryUrlMapper.details(EntryType.Artist, artist.id)}>
			<img
				src={UrlHelper.imageThumb(artist.mainPicture, ImageSize.TinyThumb)}
				alt="Thumb" /* TODO: localize */
				className="coverPicThumb"
			/>
		</a>
	);
};

export default ArtistIconLink;
