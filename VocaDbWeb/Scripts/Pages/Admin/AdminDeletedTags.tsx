import { Layout } from '@/Components/Shared/Layout';
import { TagLink } from '@/Components/Shared/Partials/Tag/TagLink';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { httpClient } from '@/Shared/HttpClient';
import { useEffect, useState } from 'react';

const AdminDeletedTags = (): React.ReactElement => {
	const title = 'Deleted tags'; /* LOC */
	const [deletedTags, setDeletedTags] = useState<TagBaseContract[] | undefined>(
		undefined,
	);

	useEffect(() => {
		httpClient
			.get<TagBaseContract[]>('/api/tags/deleted')
			.then((resp) => setDeletedTags(resp));
	}, []);

	return (
		<Layout title={title} pageTitle={title} ready>
			<ul>
				{deletedTags &&
					deletedTags.map((t) => (
						<li key={t.id}>
							<TagLink tag={t} />
						</li>
					))}
			</ul>
		</Layout>
	);
};

export default AdminDeletedTags;
