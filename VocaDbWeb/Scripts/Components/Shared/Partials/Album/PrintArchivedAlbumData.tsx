import {
	DataRow,
	DataRow_ComparedVersionsContract,
} from '@/Components/Shared/Partials/ArchivedEntry/DataRow';
import { DataRowList_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/DataRowList';
import { NameInfo } from '@/Components/Shared/Partials/ArchivedEntry/NameInfo';
import { ObjectRefInfo } from '@/Components/Shared/Partials/ArchivedEntry/ObjectRefInfo';
import { PVInfo } from '@/Components/Shared/Partials/ArchivedEntry/PVInfo';
import { PictureFileInfo } from '@/Components/Shared/Partials/ArchivedEntry/PictureFileInfo';
import { PictureRow_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/PictureRow';
import { TranslatedNameRow_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/TranslatedNameRow';
import { WebLinkInfo } from '@/Components/Shared/Partials/ArchivedEntry/WebLinkInfo';
import { ArchivedAlbumContract } from '@/DataContracts/Album/ArchivedAlbumContract';
import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface PrintArchivedAlbumDataProps {
	comparedAlbums: ComparedVersionsContract<ArchivedAlbumContract>;
}

export const PrintArchivedAlbumData = React.memo(
	({ comparedAlbums }: PrintArchivedAlbumDataProps): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Model.Resources.Albums']);

		return (
			<div className="well well-transparent archived-entry-contents">
				<h4>Content{/* TODO: localize */}</h4>

				<table className="table table-bordered">
					<tbody>
						<DataRow
							name="Album Id" /* TODO: localize */
							val={comparedAlbums.firstData.id}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<PictureRow_ComparedVersionsContract
							name="Main picture" /* TODO: localize */
							comparedVersions={comparedAlbums}
							urlGetter={(id): string =>
								`/Album/ArchivedVersionCoverPicture/${id}`
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Main picture MIME" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode => data.mainPictureMime}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<TranslatedNameRow_ComparedVersionsContract
							comparedVersions={comparedAlbums}
							valGetter={(data): ArchivedTranslatedStringContract =>
								data.translatedName
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Names" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode[] =>
								data.names?.map((name, index) => (
									<NameInfo name={name} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Description" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode => data.description}
							preserveLineBreaks={true}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Description (en)" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode => data.descriptionEng}
							preserveLineBreaks={true}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Disc type" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode =>
								t(
									`VocaDb.Model.Resources.Albums:DiscTypeNames.${data.discType}`,
								)
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Release date" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={
								(data): React.ReactNode =>
									data.originalRelease?.releaseDate?.formatted /* TODO */
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Catalog number" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode =>
								data.originalRelease?.catNum
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRow_ComparedVersionsContract
							name="Release event" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode => (
								<ObjectRefInfo objRef={data.originalRelease?.releaseEvent} />
							)}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Barcodes" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode[] =>
								data.identifiers.map((identifier) => identifier.value)
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="External links" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode[] =>
								data.webLinks?.map((webLink, index) => (
									<WebLinkInfo link={webLink} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Artists" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode[] =>
								(data.artists ?? [])
									.orderBy((artist) => artist.nameHint)
									.map(
										(artist) =>
											`${artist.nameHint} [${artist.id}] - IsSupport: ${
												artist.isSupport ? 'True' : 'False'
											}, Roles: ${artist.roles}` /* TODO: localize */,
									)
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Discs" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode[] =>
								(data.discs ?? [])
									.orderBy((disc) => disc.discNumber)
									.map(
										(disc) =>
											`${disc.discNumber}: ${disc.name} (${disc.mediaType}) [${disc.id}]` /* TODO: localize */,
									)
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Tracks" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode[] =>
								(data.songs ?? [])
									.orderBy((song) => song.discNumber)
									.orderBy((song) => song.trackNumber)
									.map(
										(song) =>
											`(Disc ${song.discNumber}) ${song.trackNumber}. ${song.nameHint} [${song.id}]` /* TODO: localize */,
									)
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="Pictures" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode[] =>
								data.pictures?.map((picture, index) => (
									<PictureFileInfo picture={picture} key={index} />
								)) ?? []
							}
						/>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<DataRowList_ComparedVersionsContract
							name="PVs" /* TODO: localize */
							comparedVersions={comparedAlbums}
							valGetter={(data): React.ReactNode[] =>
								data.pvs?.map((pv, index) => <PVInfo pv={pv} key={index} />) ??
								[]
							}
						/>
					</tbody>
				</table>
			</div>
		);
	},
);
