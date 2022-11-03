import {
	DataRow,
	DataRow_ComparedVersionsContract,
} from '@/Components/Shared/Partials/ArchivedEntry/DataRow';
import { DataRowList_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/DataRowList';
import { NameInfo } from '@/Components/Shared/Partials/ArchivedEntry/NameInfo';
import { TranslatedNameRow_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/TranslatedNameRow';
import { WebLinkInfo } from '@/Components/Shared/Partials/ArchivedEntry/WebLinkInfo';
import { regionNames } from '@/Components/regions';
import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedVenueContract } from '@/DataContracts/Venue/ArchivedVenueContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import React from 'react';

interface PrintArchivedVenueDataProps {
	comparedVenues: ComparedVersionsContract<ArchivedVenueContract>;
}

export const PrintArchivedVenueData = React.memo(
	({ comparedVenues }: PrintArchivedVenueDataProps): React.ReactElement => {
		return (
			<div className="well well-transparent archived-entry-contents">
				<h4>Content{/* LOCALIZE */}</h4>

				<table className="table table-bordered">
					<tbody>
						<DataRow
							name="Id" /* LOCALIZE */
							val={comparedVenues.firstData.id}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<TranslatedNameRow_ComparedVersionsContract
							comparedVersions={comparedVenues}
							valGetter={(data): ArchivedTranslatedStringContract =>
								data.translatedName
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Names" /* LOCALIZE */
							comparedVersions={comparedVenues}
							valGetter={(data): React.ReactNode[] =>
								data.names?.map((name, index) => (
									<NameInfo name={name} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Description" /* LOCALIZE */
							comparedVersions={comparedVenues}
							valGetter={(data): React.ReactNode => data.description}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Coordinates" /* LOCALIZE */
							comparedVersions={comparedVenues}
							valGetter={(data): React.ReactNode => data.coordinates.formatted}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Country/Region" /* LOCALIZE */
							comparedVersions={comparedVenues}
							valGetter={(data): React.ReactNode =>
								regionNames.of(data.addressCountryCode)
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Address" /* LOCALIZE */
							comparedVersions={comparedVenues}
							valGetter={(data): React.ReactNode => data.address}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="External links" /* LOCALIZE */
							comparedVersions={comparedVenues}
							valGetter={(data): React.ReactNode[] =>
								data.webLinks?.map((webLink, index) => (
									<WebLinkInfo link={webLink} key={index} />
								)) ?? []
							}
						/>
					</tbody>
				</table>
			</div>
		);
	},
);
