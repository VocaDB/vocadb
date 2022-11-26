import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { ArchivedObjectVersions } from '@/Components/Shared/Partials/ArchivedEntry/ArchivedObjectVersions';
import { CurrentVersionMessage } from '@/Components/Shared/Partials/ArchivedEntry/CurrentVersionMessage';
import { ReleaseEventSeriesForApiContract } from '@/DataContracts/ReleaseEvents/ReleaseEventSeriesForApiContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { EntryType } from '@/Models/EntryType';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

interface EventSeriesVersionsLayoutProps {
	model: EntryWithArchivedVersionsContract<ReleaseEventSeriesForApiContract>;
}

const EventSeriesVersionsLayout = ({
	model,
}: EventSeriesVersionsLayoutProps): React.ReactElement => {
	const { t, ready } = useTranslation(['ViewRes']);

	const title = `${t('ViewRes:EntryDetails.Revisions')} - ${model.entry.name}`;

	return (
		<Layout
			pageTitle={title}
			ready={ready}
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
				status={model.entry.status}
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
