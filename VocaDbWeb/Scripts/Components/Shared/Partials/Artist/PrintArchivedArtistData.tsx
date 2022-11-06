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
import { PictureFileInfo } from '@/Components/Shared/Partials/ArchivedEntry/PictureFileInfo';
import { PictureRow_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/PictureRow';
import { TranslatedNameRow_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/TranslatedNameRow';
import { WebLinkInfo } from '@/Components/Shared/Partials/ArchivedEntry/WebLinkInfo';
import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedArtistContract } from '@/DataContracts/Artist/ArchivedArtistContract';
import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface PrintArchivedArtistDataProps {
	comparedArtists: ComparedVersionsContract<ArchivedArtistContract>;
}

export const PrintArchivedArtistData = React.memo(
	({ comparedArtists }: PrintArchivedArtistDataProps): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Model.Resources']);

		return (
			<div className="well well-transparent archived-entry-contents">
				<h4>Content{/* LOC */}</h4>

				<table className="table table-bordered">
					<tbody>
						<DataRow name="Id" /* LOC */ val={comparedArtists.firstData.id} />
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<PictureRow_ComparedVersionsContract
							name="Main picture" /* LOC */
							comparedVersions={comparedArtists}
							urlGetter={(id): string => `/Artist/ArchivedVersionPicture/${id}`}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Main picture MIME" /* LOC */
							comparedVersions={comparedArtists}
							valGetter={(data): React.ReactNode => data.mainPictureMime}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<TranslatedNameRow_ComparedVersionsContract
							comparedVersions={comparedArtists}
							valGetter={(data): ArchivedTranslatedStringContract =>
								data.translatedName
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Names" /* LOC */
							comparedVersions={comparedArtists}
							valGetter={(data): React.ReactNode[] =>
								data.names?.map((name, index) => (
									<NameInfo name={name} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Description" /* LOC */
							comparedVersions={comparedArtists}
							valGetter={(data): React.ReactNode => data.description}
							preserveLineBreaks={true}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Description (en)" /* LOC */
							comparedVersions={comparedArtists}
							valGetter={(data): React.ReactNode => data.descriptionEng}
							preserveLineBreaks={true}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Release date" /* LOC */
							comparedVersions={comparedArtists}
							valGetter={(data): React.ReactNode => data.releaseDate}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Artist type" /* LOC */
							comparedVersions={comparedArtists}
							valGetter={(data): React.ReactNode =>
								t(`VocaDb.Model.Resources:ArtistTypeNames.${data.artistType}`)
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="External links" /* LOC */
							comparedVersions={comparedArtists}
							valGetter={(data): React.ReactNode[] =>
								data.webLinks?.map((webLink, index) => (
									<WebLinkInfo link={webLink} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Base voicebank" /* LOC */
							comparedVersions={comparedArtists}
							valGetter={(data): React.ReactNode => (
								<ObjectRefInfo objRef={data.baseVoicebank} />
							)}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Associated artists" /* LOC */
							comparedVersions={comparedArtists}
							valGetter={(data): React.ReactNode[] =>
								data.groups.map(
									(group) =>
										`${group.nameHint} [${group.id}] (${group.linkType})`,
								)
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<ObjectRefList_ComparedVersionsContract
							name="Members" /* LOC */
							comparedVersions={comparedArtists}
							valGetter={(data): ObjectRefContract[] => data.members}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Pictures" /* LOC */
							comparedVersions={comparedArtists}
							valGetter={(data): React.ReactNode[] =>
								data.pictures?.map((picture, index) => (
									<PictureFileInfo picture={picture} key={index} />
								)) ?? []
							}
						/>
					</tbody>
				</table>
			</div>
		);
	},
);
