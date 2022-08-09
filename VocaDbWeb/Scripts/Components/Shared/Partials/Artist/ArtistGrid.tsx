import { ArtistIconLink } from '@/Components/Shared/Partials/Artist/ArtistIconLink';
import { ArtistLink } from '@/Components/Shared/Partials/Artist/ArtistLink';
import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import _ from 'lodash';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ArtistGridProps {
	artists: ArtistApiContract[];
	columns: number;
	displayType?: boolean;
	tooltip?: boolean;
}

export const ArtistGrid = ({
	artists,
	columns,
	displayType = false,
	tooltip = false,
}: ArtistGridProps): React.ReactElement => {
	const { t } = useTranslation(['VocaDb.Model.Resources']);

	return (
		<table>
			<tbody>
				{_.chain(artists)
					.chunk(columns)
					.value()
					.map((chunk, index) => (
						<tr key={index}>
							{chunk.map((artist) => (
								<React.Fragment key={artist.id}>
									<td>
										<ArtistIconLink artist={artist} />
									</td>
									<td>
										<ArtistLink artist={artist} tooltip={tooltip} />
										{displayType && (
											<>
												{' '}
												(
												{t(
													`VocaDb.Model.Resources:ArtistTypeNames.${artist.artistType}`,
												)}
												)
											</>
										)}
										{artist.additionalNames && (
											<>
												<br />
												<span className="extraInfo">
													{artist.additionalNames}
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
