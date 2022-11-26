import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { ArchivedObjectVersions } from '@/Components/Shared/Partials/ArchivedEntry/ArchivedObjectVersions';
import { CurrentVersionMessage } from '@/Components/Shared/Partials/ArchivedEntry/CurrentVersionMessage';
import { SongListContract } from '@/DataContracts/Song/SongListContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { EntryType } from '@/Models/EntryType';
import { songListRepo } from '@/Repositories/SongListRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

interface SongListVersionsLayoutProps {
	model: EntryWithArchivedVersionsContract<SongListContract>;
}

const SongListVersionsLayout = ({
	model,
}: SongListVersionsLayoutProps): React.ReactElement => {
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
				status={model.entry.status}
			/>

			<ArchivedObjectVersions
				archivedVersions={model.archivedVersions}
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
