import { StarsMetaSpan } from '@/Components/Shared/Partials/Shared/StarsMetaSpan';
import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { AlbumType } from '@/Models/Albums/AlbumType';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface AlbumPopupContentProps {
	album: AlbumContract;
}

export const AlbumPopupContent = React.memo(
	({ album }: AlbumPopupContentProps): React.ReactElement => {
		const { t } = useTranslation([
			'HelperRes',
			'ViewRes.Album',
			'VocaDb.Model.Resources.Albums',
		]);

		return (
			<>
				<p>
					<strong className="popupTitle">{album.name}</strong>
					{album.additionalNames && (
						<>
							<br />
							{album.additionalNames}
						</>
					)}
				</p>
				<p>
					{album.artistString}
					<br />
					{album.discType !== AlbumType.Unknown &&
						t(`VocaDb.Model.Resources.Albums:DiscTypeNames.${album.discType}`)}
				</p>
				{!album.releaseDate.isEmpty && (
					<p>
						{t('HelperRes:AlbumHelpers.Released')} {album.releaseDate.formatted}
						{album.releaseEvent && <> ({album.releaseEvent.name})</>}
					</p>
				)}
				{album.ratingCount > 0 && (
					<>
						<StarsMetaSpan current={album.ratingAverage} max={5} /> (
						{album.ratingCount} {t('ViewRes.Album:Details.Ratings')})
					</>
				)}
			</>
		);
	},
);
