import Breadcrumb from '@Bootstrap/Breadcrumb';
import VenueForApiContract from '@DataContracts/Venue/VenueForApiContract';
import EntryWithArchivedVersionsContract from '@DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import VenueRepository from '@Repositories/VenueRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

import Layout from '../../Components/Shared/Layout';
import ArchivedObjectVersions from '../../Components/Shared/Partials/ArchivedEntry/ArchivedObjectVersions';
import CurrentVersionMessage from '../../Components/Shared/Partials/ArchivedEntry/CurrentVersionMessage';
import useVocaDbTitle from '../../Components/useVocaDbTitle';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);
const venueRepo = new VenueRepository(httpClient, urlMapper);

interface VenueVersionsLayoutProps {
	model: EntryWithArchivedVersionsContract<VenueForApiContract>;
}

const VenueVersionsLayout = ({
	model,
}: VenueVersionsLayoutProps): React.ReactElement => {
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
							to: '/Event/EventsByVenue',
						}}
						divider
					>
						{t(`ViewRes:Shared.Venues`)}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: EntryUrlMapper.details(EntryType.Venue, model.entry.id),
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
				linkFunc={(id): string => `/Venue/ViewVersion/${id}`}
				entryType={EntryType.Venue}
			/>
		</Layout>
	);
};

const VenueVersions = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<
		EntryWithArchivedVersionsContract<VenueForApiContract>
	>();

	React.useEffect(() => {
		venueRepo
			.getVenueWithArchivedVersions({ id: Number(id) })
			.then((model) => setModel(model));
	}, [id]);

	return model ? <VenueVersionsLayout model={model} /> : <></>;
};

export default VenueVersions;
