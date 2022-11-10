import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { ArchivedObjectVersions } from '@/Components/Shared/Partials/ArchivedEntry/ArchivedObjectVersions';
import { CurrentVersionMessage } from '@/Components/Shared/Partials/ArchivedEntry/CurrentVersionMessage';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { EntryType } from '@/Models/EntryType';
import { songRepo } from '@/Repositories/SongRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

interface SongVersionsLayoutProps {
	model: EntryWithArchivedVersionsContract<SongApiContract>;
}

const SongVersionsLayout = ({
	model,
}: SongVersionsLayoutProps): React.ReactElement => {
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
							to: '/Song',
						}}
						divider
					>
						{t(`ViewRes:Shared.Songs`)}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: EntryUrlMapper.details(EntryType.Song, model.entry.id),
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
				linkFunc={(id): string => `/Song/ViewVersion/${id}`}
				entryType={EntryType.Song}
			/>
		</Layout>
	);
};

const SongVersions = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<
		EntryWithArchivedVersionsContract<SongApiContract>
	>();

	React.useEffect(() => {
		songRepo
			.getSongWithArchivedVersions({ id: Number(id) })
			.then((model) => setModel(model));
	}, [id]);

	return model ? <SongVersionsLayout model={model} /> : <></>;
};

export default SongVersions;
