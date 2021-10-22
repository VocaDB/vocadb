import AlbumForApiContract from '@DataContracts/Album/AlbumForApiContract';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import _ from 'lodash';
import React from 'react';
import { useTranslation } from 'react-i18next';

import AlbumIconLink from './AlbumIconLink';

interface AlbumGridProps {
	albums: AlbumForApiContract[];
	columns: number;
	displayRating: boolean;
	displayReleaseDate: boolean;
	displayType?: boolean;
}

const AlbumGrid = ({
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
				{_.chain(albums)
					.chunk(columns)
					.value()
					.map((chunk, index) => (
						<tr key={index}>
							{chunk.map((album) => (
								<React.Fragment key={album.id}>
									<td>
										<AlbumIconLink album={album} />
									</td>
									<td>
										<a
											href={EntryUrlMapper.details(EntryType.Album, album.id)}
											title={album.additionalNames}
										>
											{album.name}
										</a>
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
													{album.releaseDate.formatted}
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

export default AlbumGrid;
