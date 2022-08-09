import { SongTypeLabel } from '@/Components/Shared/Partials/Song/SongTypeLabel';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import _ from 'lodash';
import moment from 'moment';
import React from 'react';
import { Link } from 'react-router-dom';

interface SongIconLinkProps {
	song: SongApiContract;
}

const SongIconLink = ({ song }: SongIconLinkProps): React.ReactElement => {
	return song.mainPicture && song.mainPicture.urlThumb ? (
		<Link to={EntryUrlMapper.details_song(song)}>
			<img
				src={UrlHelper.upgradeToHttps(song.mainPicture.urlThumb)}
				alt="Cover" /* TODO: localize */
				className="coverPicThumb"
				referrerPolicy="same-origin"
			/>
		</Link>
	) : (
		<></>
	);
};

interface SongLinkProps {
	song: SongApiContract;
}

const SongLink = ({ song }: SongLinkProps): React.ReactElement => {
	return <Link to={`${EntryUrlMapper.details_song(song)}`}>{song.name}</Link>;
};

interface SongGridProps {
	songs: SongApiContract[];
	columns: number;
	displayType?: boolean;
	displayPublishDate?: boolean;
}

export const SongGrid = ({
	songs,
	columns,
	displayType = false,
	displayPublishDate = false,
}: SongGridProps): React.ReactElement => {
	return (
		<table>
			<tbody>
				{_.chain(songs)
					.chunk(columns)
					.value()
					.map((chunk, index) => (
						<tr key={index}>
							{chunk.map((song) => (
								<React.Fragment key={song.id}>
									<td>
										<SongIconLink song={song} />
									</td>
									<td>
										<SongLink song={song} />
										{displayType && (
											<>
												{' '}
												<SongTypeLabel songType={song.songType} />
											</>
										)}
										{displayPublishDate && song.publishDate && (
											<>
												{' '}
												<i
													className="icon-calendar"
													title={`Published: ${
														moment(song.publishDate)
															.utc()
															.format('l') /* REVIEW */
													}`} /* TODO: localize */
													/* TODO: tooltip */
												/>
											</>
										)}
										<br />
										<span className="extraInfo">{song.artistString}</span>
									</td>
								</React.Fragment>
							))}
						</tr>
					))}
			</tbody>
		</table>
	);
};
