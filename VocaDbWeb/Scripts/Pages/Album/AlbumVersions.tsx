import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { ArchivedObjectVersions } from '@/Components/Shared/Partials/ArchivedEntry/ArchivedObjectVersions';
import { CurrentVersionMessage } from '@/Components/Shared/Partials/ArchivedEntry/CurrentVersionMessage';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { EntryType } from '@/Models/EntryType';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

const httpClient = new HttpClient();
const albumRepo = new AlbumRepository(httpClient, vdb.values.baseAddress);

interface AlbumVersionsLayoutProps {
	model: EntryWithArchivedVersionsContract<AlbumForApiContract>;
}

const AlbumVersionsLayout = ({
	model,
}: AlbumVersionsLayoutProps): React.ReactElement => {
	const { t, ready } = useTranslation(['ViewRes']);

	const title = `${t('ViewRes:EntryDetails.Revisions')} - ${model.entry.name}`;

	useVdbTitle(title, ready);

	return (
		<Layout
			title={title}
			parents={
				<>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: '/Album',
						}}
						divider
					>
						{t(`ViewRes:Shared.Albums`)}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: EntryUrlMapper.details(EntryType.Album, model.entry.id),
						}}
					>
						{model.entry.name}
					</Breadcrumb.Item>
				</>
			}
		>
			<CurrentVersionMessage
				version={model.entry.version}
				status={model.entry.status}
			/>

			<ArchivedObjectVersions
				archivedVersions={model.archivedVersions}
				linkFunc={(id): string => `/Album/ViewVersion/${id}`}
				entryType={EntryType.Album}
			/>
		</Layout>
	);
};

const AlbumVersions = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<
		EntryWithArchivedVersionsContract<AlbumForApiContract>
	>();

	React.useEffect(() => {
		albumRepo
			.getAlbumWithArchivedVersions({ id: Number(id) })
			.then((model) => setModel(model));
	}, [id]);

	return model ? <AlbumVersionsLayout model={model} /> : <></>;
};

export default AlbumVersions;
