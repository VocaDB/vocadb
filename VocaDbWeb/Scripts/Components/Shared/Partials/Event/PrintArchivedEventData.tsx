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
import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedEventContract } from '@/DataContracts/ReleaseEvents/ArchivedEventContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import React from 'react';

interface PrintArchivedEventDataProps {
	comparedEvents: ComparedVersionsContract<ArchivedEventContract>;
}

export const PrintArchivedEventData = React.memo(
	({ comparedEvents }: PrintArchivedEventDataProps): React.ReactElement => {
		return (
			<div className="well well-transparent archived-entry-contents">
				<h4>Content{/* LOC */}</h4>

				<table className="table table-bordered">
					<tbody>
						<DataRow name="Id" /* LOC */ val={comparedEvents.firstData.id} />
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<TranslatedNameRow_ComparedVersionsContract
							comparedVersions={comparedEvents}
							valGetter={(data): ArchivedTranslatedStringContract =>
								data.translatedName
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Names" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode[] =>
								data.names?.map((name, index) => (
									<NameInfo name={name} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Description" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.description}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="External links" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode[] =>
								data.webLinks?.map((webLink, index) => (
									<WebLinkInfo link={webLink} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Category" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.category}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Date" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.date}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Venue" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => (
								<ObjectRefInfo objRef={data.venue} />
							)}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Venue name" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.venueName}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Main picture MIME" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.mainPictureMime}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Series" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => (
								<ObjectRefInfo objRef={data.series} />
							)}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Series number" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.seriesNumber}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Artists" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode[] =>
								data.artists?.map((artist, index) => (
									<ObjectRefInfo objRef={artist} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="PVs" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode[] =>
								data.pvs?.map((pv, index) => <PVInfo pv={pv} key={index} />) ??
								[]
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Song list" /* LOC */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => (
								<ObjectRefInfo objRef={data.songList} />
							)}
						/>
					</tbody>
				</table>
			</div>
		);
	},
);
