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
				<h4>Content{/* TODO: localize */}</h4>

				<table className="table table-bordered">
					<tbody>
						<DataRow
							name="Id" /* TODO: localize */
							val={comparedEvents.firstData.id}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<TranslatedNameRow_ComparedVersionsContract
							comparedVersions={comparedEvents}
							valGetter={(data): ArchivedTranslatedStringContract =>
								data.translatedName
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Names" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode[] =>
								data.names?.map((name, index) => (
									<NameInfo name={name} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Description" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.description}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="External links" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode[] =>
								data.webLinks?.map((webLink, index) => (
									<WebLinkInfo link={webLink} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Category" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.category}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Date" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.date}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Venue" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => (
								<ObjectRefInfo objRef={data.venue} />
							)}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Venue name" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.venueName}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Main picture MIME" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.mainPictureMime}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Series" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => (
								<ObjectRefInfo objRef={data.series} />
							)}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Series number" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode => data.seriesNumber}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Artists" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode[] =>
								data.artists?.map((artist, index) => (
									<ObjectRefInfo objRef={artist} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="PVs" /* TODO: localize */
							comparedVersions={comparedEvents}
							valGetter={(data): React.ReactNode[] =>
								data.pvs?.map((pv, index) => <PVInfo pv={pv} key={index} />) ??
								[]
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Song list" /* TODO: localize */
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
