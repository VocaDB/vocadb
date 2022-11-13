import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { ArchivedObjectVersions } from '@/Components/Shared/Partials/ArchivedEntry/ArchivedObjectVersions';
import { CurrentVersionMessage } from '@/Components/Shared/Partials/ArchivedEntry/CurrentVersionMessage';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { EntryType } from '@/Models/EntryType';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

interface ArtistVersionsLayoutProps {
	model: EntryWithArchivedVersionsContract<ArtistApiContract>;
}

const ArtistVersionsLayout = ({
	model,
}: ArtistVersionsLayoutProps): React.ReactElement => {
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
							to: '/Artist',
						}}
						divider
					>
						{t(`ViewRes:Shared.Artists`)}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: EntryUrlMapper.details(EntryType.Artist, model.entry.id),
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
				linkFunc={(id): string => `/Artist/ViewVersion/${id}`}
				entryType={EntryType.Artist}
			/>
		</Layout>
	);
};

const ArtistVersions = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<
		EntryWithArchivedVersionsContract<ArtistApiContract>
	>();

	React.useEffect(() => {
		artistRepo
			.getArtistWithArchivedVersions({ id: Number(id) })
			.then((model) => setModel(model));
	}, [id]);

	return model ? <ArtistVersionsLayout model={model} /> : <></>;
};

export default ArtistVersions;
