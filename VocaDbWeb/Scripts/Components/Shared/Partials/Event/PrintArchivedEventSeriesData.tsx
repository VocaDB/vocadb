import {
	DataRow,
	DataRow_ComparedVersionsContract,
} from '@/Components/Shared/Partials/ArchivedEntry/DataRow';
import { DataRowList_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/DataRowList';
import { NameInfo } from '@/Components/Shared/Partials/ArchivedEntry/NameInfo';
import { TranslatedNameRow_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/TranslatedNameRow';
import { WebLinkInfo } from '@/Components/Shared/Partials/ArchivedEntry/WebLinkInfo';
import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedEventSeriesContract } from '@/DataContracts/ReleaseEvents/ArchivedEventSeriesContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import React from 'react';

interface PrintArchivedEventSeriesDataProps {
	comparedSeries: ComparedVersionsContract<ArchivedEventSeriesContract>;
}

export const PrintArchivedEventSeriesData = React.memo(
	({
		comparedSeries,
	}: PrintArchivedEventSeriesDataProps): React.ReactElement => {
		return (
			<div className="well well-transparent archived-entry-contents">
				<h4>Content{/* LOCALIZE */}</h4>

				<table className="table table-bordered">
					<tbody>
						<DataRow
							name="Id" /* LOCALIZE */
							val={comparedSeries.firstData.id}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<TranslatedNameRow_ComparedVersionsContract
							comparedVersions={comparedSeries}
							valGetter={(data): ArchivedTranslatedStringContract =>
								data.translatedName
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Names" /* LOCALIZE */
							comparedVersions={comparedSeries}
							valGetter={(data): React.ReactNode[] =>
								data.names?.map((name, index) => (
									<NameInfo name={name} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Description" /* LOCALIZE */
							comparedVersions={comparedSeries}
							valGetter={(data): React.ReactNode => data.description}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="External links" /* LOCALIZE */
							comparedVersions={comparedSeries}
							valGetter={(data): React.ReactNode[] =>
								data.webLinks?.map((webLink, index) => (
									<WebLinkInfo link={webLink} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Category" /* LOCALIZE */
							comparedVersions={comparedSeries}
							valGetter={(data): React.ReactNode => data.category}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Main picture MIME" /* LOCALIZE */
							comparedVersions={comparedSeries}
							valGetter={(data): React.ReactNode => data.mainPictureMime}
						/>
					</tbody>
				</table>
			</div>
		);
	},
);
