import {
	DataRow,
	DataRow_ComparedVersionsContract,
} from '@/Components/Shared/Partials/ArchivedEntry/DataRow';
import { DataRowList_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/DataRowList';
import { NameInfo } from '@/Components/Shared/Partials/ArchivedEntry/NameInfo';
import { ObjectRefInfo } from '@/Components/Shared/Partials/ArchivedEntry/ObjectRefInfo';
import { PVInfo } from '@/Components/Shared/Partials/ArchivedEntry/PVInfo';
import { TranslatedNameRow_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/TranslatedNameRow';
import { WebLinkInfo } from '@/Components/Shared/Partials/ArchivedEntry/WebLinkInfo';
import { LyricsInfo } from '@/Components/Shared/Partials/Song/LyricsInfo';
import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedSongContract } from '@/DataContracts/Song/ArchivedSongContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import { BpmHelper } from '@/Helpers/BpmHelper';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import React from 'react';

interface PrintArchivedSongDataProps {
	comparedSongs: ComparedVersionsContract<ArchivedSongContract>;
}

export const PrintArchivedSongData = React.memo(
	({ comparedSongs }: PrintArchivedSongDataProps): React.ReactElement => {
		return (
			<div className="well well-transparent archived-entry-contents">
				<h4>Content{/* LOC */}</h4>

				<table className="table table-bordered">
					<tbody>
						<DataRow name="Id" /* LOC */ val={comparedSongs.firstData.id} />
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<TranslatedNameRow_ComparedVersionsContract
							comparedVersions={comparedSongs}
							valGetter={(data): ArchivedTranslatedStringContract =>
								data.translatedName
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Names" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode[] =>
								data.names?.map((name, index) => (
									<NameInfo name={name} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Notes" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode => data.notes}
							preserveLineBreaks={true}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Notes (en)" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode => data.notesEng}
							preserveLineBreaks={true}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Song type" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode => data.songType}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Original version" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode => (
								<ObjectRefInfo objRef={data.originalVersion} />
							)}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Duration" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode =>
								DateTimeHelper.formatFromSeconds(data.lengthSeconds)
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="BPM" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode =>
								BpmHelper.formatFromMilliBpm(data.minMilliBpm, data.maxMilliBpm)
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Release events" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode[] | React.ReactNode => {
								if (data.releaseEvent !== undefined) {
									return <ObjectRefInfo objRef={data.releaseEvent} />;
								}
								return data.releaseEvents!.map((e, index) => (
									<ObjectRefInfo objRef={e} key={index} />
								));
							}}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Publish date" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode => data.publishDate}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="External links" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode[] =>
								data.webLinks?.map((webLink, index) => (
									<WebLinkInfo link={webLink} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Artists" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode[] =>
								(data.artists ?? [])
									.orderBy((artist) => artist.nameHint)
									.map(
										(artist) =>
											`${artist.nameHint} [${artist.id}] - IsSupport: ${
												artist.isSupport ? 'True' : 'False'
											}, Roles: ${artist.roles}` /* LOC */,
									)
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="PVs" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode[] =>
								data.pvs?.map((pv, index) => <PVInfo pv={pv} key={index} />) ??
								[]
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Lyrics" /* LOC */
							comparedVersions={comparedSongs}
							valGetter={(data): React.ReactNode[] =>
								data.lyrics?.map((l, index) => (
									<LyricsInfo lyrics={l} key={index} />
								)) ?? []
							}
						/>
					</tbody>
				</table>
			</div>
		);
	},
);
