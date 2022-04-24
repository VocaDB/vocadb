import Breadcrumb from '@Bootstrap/Breadcrumb';
import ReleaseEventSeriesForApiContract from '@DataContracts/ReleaseEvents/ReleaseEventSeriesForApiContract';
import EntryWithArchivedVersionsContract from '@DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import ReleaseEventRepository from '@Repositories/ReleaseEventRepository';
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
const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);

interface EventSeriesVersionsLayoutProps {
	model: EntryWithArchivedVersionsContract<ReleaseEventSeriesForApiContract>;
}

const EventSeriesVersionsLayout = ({
	model,
}: EventSeriesVersionsLayoutProps): React.ReactElement => {
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
							to: '/Event',
						}}
						divider
					>
						{t(`ViewRes:Shared.ReleaseEvents`)}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: EntryUrlMapper.details(
								EntryType.ReleaseEventSeries,
								model.entry.id,
							),
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
				linkFunc={(id): string => `/Event/ViewSeriesVersion/${id}`}
				entryType={EntryType.ReleaseEventSeries}
			/>
		</Layout>
	);
};

const EventSeriesVersions = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<
		EntryWithArchivedVersionsContract<ReleaseEventSeriesForApiContract>
	>();

	React.useEffect(() => {
		eventRepo
			.getReleaseEventSeriesWithArchivedVersions({ id: Number(id) })
			.then((model) => setModel(model));
	}, [id]);

	return model ? <EventSeriesVersionsLayout model={model} /> : <></>;
};

export default EventSeriesVersions;
