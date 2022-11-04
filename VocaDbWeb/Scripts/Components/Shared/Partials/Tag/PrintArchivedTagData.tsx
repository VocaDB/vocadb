import {
	DataRow,
	DataRow_ComparedVersionsContract,
} from '@/Components/Shared/Partials/ArchivedEntry/DataRow';
import { DataRowList_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/DataRowList';
import { NameInfo } from '@/Components/Shared/Partials/ArchivedEntry/NameInfo';
import {
	ObjectRefInfo,
	ObjectRefList_ComparedVersionsContract,
} from '@/Components/Shared/Partials/ArchivedEntry/ObjectRefInfo';
import { TranslatedNameRow_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/TranslatedNameRow';
import { WebLinkInfo } from '@/Components/Shared/Partials/ArchivedEntry/WebLinkInfo';
import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { ArchivedTagContract } from '@/DataContracts/Tag/ArchivedTagContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import React from 'react';

interface PrintArchivedTagDataProps {
	comparedTags: ComparedVersionsContract<ArchivedTagContract>;
}

export const PrintArchivedTagData = React.memo(
	({ comparedTags }: PrintArchivedTagDataProps): React.ReactElement => {
		return (
			<div className="well well-transparent archived-entry-contents">
				<h4>Content{/* LOC */}</h4>

				<table className="table table-bordered">
					<tbody>
						<DataRow name="Id" /* LOC */ val={comparedTags.firstData.id} />
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<TranslatedNameRow_ComparedVersionsContract
							comparedVersions={comparedTags}
							valGetter={(data): ArchivedTranslatedStringContract =>
								data.translatedName
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Names" /* LOC */
							comparedVersions={comparedTags}
							valGetter={(data): React.ReactNode[] =>
								data.names?.map((name, index) => (
									<NameInfo name={name} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Description" /* LOC */
							comparedVersions={comparedTags}
							valGetter={(data): React.ReactNode => data.description}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Description English" /* LOC */
							comparedVersions={comparedTags}
							valGetter={(data): React.ReactNode => data.descriptionEng}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<ObjectRefList_ComparedVersionsContract
							name="Related tags" /* LOC */
							comparedVersions={comparedTags}
							valGetter={(data): ObjectRefContract[] => data.relatedTags ?? []}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Valid for" /* LOC */
							comparedVersions={comparedTags}
							valGetter={(data): React.ReactNode => data.targets}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="External links" /* LOC */
							comparedVersions={comparedTags}
							valGetter={(data): React.ReactNode[] =>
								data.webLinks?.map((webLink, index) => (
									<WebLinkInfo link={webLink} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Category name" /* LOC */
							comparedVersions={comparedTags}
							valGetter={(data): React.ReactNode => data.categoryName}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Parent" /* LOC */
							comparedVersions={comparedTags}
							valGetter={(data): React.ReactNode => (
								<ObjectRefInfo objRef={data.parent} />
							)}
						/>
					</tbody>
				</table>
			</div>
		);
	},
);
