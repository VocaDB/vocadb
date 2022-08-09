import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { ArchivedObjectVersions } from '@/Components/Shared/Partials/ArchivedEntry/ArchivedObjectVersions';
import { CurrentVersionMessage } from '@/Components/Shared/Partials/ArchivedEntry/CurrentVersionMessage';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);
const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);

interface EventVersionsLayoutProps {
	model: EntryWithArchivedVersionsContract<ReleaseEventContract>;
}

const EventVersionsLayout = ({
	model,
}: EventVersionsLayoutProps): React.ReactElement => {
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
								EntryType.ReleaseEvent,
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
				linkFunc={(id): string => `/Event/ViewVersion/${id}`}
				entryType={EntryType.ReleaseEvent}
			/>
		</Layout>
	);
};

const EventVersions = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<
		EntryWithArchivedVersionsContract<ReleaseEventContract>
	>();

	React.useEffect(() => {
		eventRepo
			.getReleaseEventWithArchivedVersions({ id: Number(id) })
			.then((model) => setModel(model));
	}, [id]);

	return model ? <EventVersionsLayout model={model} /> : <></>;
};

export default EventVersions;
