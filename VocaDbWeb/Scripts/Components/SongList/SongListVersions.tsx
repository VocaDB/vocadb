import Breadcrumb from '@Bootstrap/Breadcrumb';
import SongListContract from '@DataContracts/Song/SongListContract';
import EntryWithArchivedVersionsContract from '@DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import SongListRepository from '@Repositories/SongListRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

import Layout from '../Shared/Layout';
import ArchivedObjectVersions from '../Shared/Partials/ArchivedEntry/ArchivedObjectVersions';
import CurrentVersionMessage from '../Shared/Partials/ArchivedEntry/CurrentVersionMessage';
import useVocaDbTitle from '../useVocaDbTitle';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);
const songListRepo = new SongListRepository(httpClient, urlMapper);

interface SongListVersionsLayoutProps {
	model: EntryWithArchivedVersionsContract<SongListContract>;
}

const SongListVersionsLayout = ({
	model,
}: SongListVersionsLayoutProps): React.ReactElement => {
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
							to: EntryUrlMapper.details(EntryType.SongList, model.entry.id),
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
				linkFunc={(id): string => `/SongList/ViewVersion/${id}`}
				entryType={EntryType.SongList}
			/>
		</Layout>
	);
};

const SongListVersions = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<
		EntryWithArchivedVersionsContract<SongListContract>
	>();

	React.useEffect(() => {
		songListRepo
			.getSongListWithArchivedVersions({ id: Number(id) })
			.then((model) => setModel(model));
	}, [id]);

	return model ? <SongListVersionsLayout model={model} /> : <></>;
};

export default SongListVersions;
