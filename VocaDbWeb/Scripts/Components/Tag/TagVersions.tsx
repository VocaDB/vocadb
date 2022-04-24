import Breadcrumb from '@Bootstrap/Breadcrumb';
import TagApiContract from '@DataContracts/Tag/TagApiContract';
import EntryWithArchivedVersionsContract from '@DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import TagRepository from '@Repositories/TagRepository';
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
const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);

interface TagVersionsLayoutProps {
	model: EntryWithArchivedVersionsContract<TagApiContract>;
}

const TagVersionsLayout = ({
	model,
}: TagVersionsLayoutProps): React.ReactElement => {
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
							to: '/Tag',
						}}
						divider
					>
						{t(`ViewRes:Shared.Tags`)}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: EntryUrlMapper.details(EntryType.Tag, model.entry.id),
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
				linkFunc={(id): string => `/Tag/ViewVersion/${id}`}
				entryType={EntryType.Tag}
			/>
		</Layout>
	);
};

const TagVersions = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<
		EntryWithArchivedVersionsContract<TagApiContract>
	>();

	React.useEffect(() => {
		tagRepo
			.getTagWithArchivedVersions({ id: Number(id) })
			.then((model) => setModel(model));
	}, [id]);

	return model ? <TagVersionsLayout model={model} /> : <></>;
};

export default TagVersions;
