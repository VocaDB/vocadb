import SongApiContract from '@DataContracts/Song/SongApiContract';
import UrlHelper from '@Helpers/UrlHelper';
import SongType from '@Models/Songs/SongType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import _ from 'lodash';
import moment from 'moment';
import React from 'react';

import SongTypeLabel from './SongTypeLabel';

interface SongIconLinkProps {
	song: SongApiContract;
}

const SongIconLink = ({ song }: SongIconLinkProps): React.ReactElement => {
	return (
		<a href={EntryUrlMapper.details_song(song)}>
			<img
				src={UrlHelper.upgradeToHttps(song.thumbUrl)}
				alt="Cover" /* TODO: localize */
				className="coverPicThumb"
				referrerPolicy="same-origin"
			/>
		</a>
	);
};

interface SongLinkProps {
	song: SongApiContract;
}

const SongLink = ({ song }: SongLinkProps): React.ReactElement => {
	return <a href={`${EntryUrlMapper.details_song(song)}`}>{song.name}</a>;
};

interface SongGridProps {
	songs: SongApiContract[];
	columns: number;
	displayType?: boolean;
	displayPublishDate?: boolean;
}

const SongGrid = ({
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
									<td>{song.thumbUrl && <SongIconLink song={song} />}</td>
									<td>
										<SongLink song={song} />
										{displayType && (
											<>
												{' '}
												<SongTypeLabel
													songType={
														SongType[song.songType as keyof typeof SongType]
													}
												/>
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

export default SongGrid;
