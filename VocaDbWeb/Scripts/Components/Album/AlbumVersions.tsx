import Breadcrumb from '@Bootstrap/Breadcrumb';
import AlbumForApiContract from '@DataContracts/Album/AlbumForApiContract';
import EntryWithArchivedVersionsContract from '@DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import AlbumRepository from '@Repositories/AlbumRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import HttpClient from '@Shared/HttpClient';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

import Layout from '../Shared/Layout';
import ArchivedObjectVersions from '../Shared/Partials/ArchivedEntry/ArchivedObjectVersions';
import CurrentVersionMessage from '../Shared/Partials/ArchivedEntry/CurrentVersionMessage';
import useVocaDbTitle from '../useVocaDbTitle';

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

	useVocaDbTitle(title, ready);

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
				status={EntryStatus[model.entry.status as keyof typeof EntryStatus]}
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
