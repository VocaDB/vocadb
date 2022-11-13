import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { ImageSize } from '@/Models/Images/ImageSize';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ArtistPopupContentProps {
	artist: ArtistContract;
}

export const ArtistPopupContent = React.memo(
	({ artist }: ArtistPopupContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Artist', 'VocaDb.Model.Resources']);

		return (
			<>
				<div className="thumbnail">
					<img
						src={UrlHelper.imageThumb(artist.mainPicture, ImageSize.TinyThumb)}
						alt="Thumb" /* LOC */
						className="coverPicThumb"
					/>
				</div>

				<p>
					<strong className="popupTitle">{artist.name}</strong>

					{artist.additionalNames && (
						<>
							<br />
							{artist.additionalNames}
						</>
					)}
				</p>

				<p>
					{t(`VocaDb.Model.Resources:ArtistTypeNames.${artist.artistType}`)}
				</p>

				{artist.releaseDate && (
					<p>
						{t('ViewRes.Artist:Details.ReleaseDate')}{' '}
						{DateTimeHelper.DateOnly_utc_format_l(artist.releaseDate)}
					</p>
				)}
			</>
		);
	},
);
