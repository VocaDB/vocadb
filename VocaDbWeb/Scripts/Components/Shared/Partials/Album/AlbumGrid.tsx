import { AlbumIconLink } from '@/Components/Shared/Partials/Album/AlbumIconLink';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface AlbumGridProps {
	albums: AlbumForApiContract[];
	columns: number;
	displayRating: boolean;
	displayReleaseDate: boolean;
	displayType?: boolean;
}

export const AlbumGrid = ({
	albums,
	columns,
	displayRating,
	displayReleaseDate,
	displayType = false,
}: AlbumGridProps): React.ReactElement => {
	const { t } = useTranslation(['HelperRes', 'VocaDb.Model.Resources.Albums']);

	return (
		<table>
			<tbody>
				{albums.chunk(columns).map((chunk, index) => (
					<tr key={index}>
						{chunk.map((album) => (
							<React.Fragment key={album.id}>
								<td>
									<AlbumIconLink album={album} />
								</td>
								<td>
									<Link
										to={EntryUrlMapper.details(EntryType.Album, album.id)}
										title={album.additionalNames}
									>
										{album.name}
									</Link>
									{displayType && (
										<>
											{' '}
											(
											{t(
												`VocaDb.Model.Resources.Albums:DiscTypeNames.${album.discType}`,
											)}
											)
										</>
									)}
									<br />
									<span className="extraInfo">{album.artistString}</span>
									{displayReleaseDate && !album.releaseDate.isEmpty && (
										<>
											<br />
											<span>
												{t('HelperRes:AlbumHelpers.Released')}{' '}
												{DateTimeHelper.formatComponentDate(
													album.releaseDate.year,
													album.releaseDate.month,
													album.releaseDate.day,
												)}
											</span>
										</>
									)}
								</td>
							</React.Fragment>
						))}
					</tr>
				))}
			</tbody>
		</table>
	);
};
