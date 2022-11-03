import { SongContract } from '@/DataContracts/Song/SongContract';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import moment from 'moment';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SongPopupContentProps {
	song: SongContract;
}

export const SongPopupContent = React.memo(
	({ song }: SongPopupContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'VocaDb.Model.Resources.Songs']);

		return (
			<>
				{song.thumbUrl && (
					<div className="thumbnail">
						<img
							src={song.thumbUrl}
							alt="Thumb" /* LOCALIZE */
							className="coverPicThumb"
							referrerPolicy="same-origin"
						/>
					</div>
				)}

				<strong className="popupTitle">{song.name}</strong>

				{song.additionalNames && <p>{song.additionalNames}</p>}

				<p>
					{song.artistString}
					<br />
					{t(`VocaDb.Model.Resources.Songs:SongTypeNames.${song.songType}`)}
				</p>

				{song.publishDate && (
					<p>
						{t('ViewRes:EntryDetails.PublishDate')}{' '}
						{moment(song.publishDate).format('l')}
					</p>
				)}

				{song.lengthSeconds > 0 && (
					<p>{DateTimeHelper.formatFromSeconds(song.lengthSeconds)}</p>
				)}
			</>
		);
	},
);
