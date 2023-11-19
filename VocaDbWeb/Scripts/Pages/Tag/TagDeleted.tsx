import { Layout } from '@/Components/Shared/Layout';
import { tagRepo } from '@/Repositories/TagRepository';
import { DeletedTagsStore } from '@/Stores/Tag/DeletedTagsStore';
import { useVdb } from '@/VdbContext';
import { useLocationStateStore } from '@/route-sphere';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';

import TagSearchList from '../Search/Partials/TagSearchList';

const AdminDeletedTags = observer((): React.ReactElement => {
	const vdb = useVdb();

	const [deletedTagsStore] = React.useState(
		() => new DeletedTagsStore(vdb.values, tagRepo),
	);

	const title = 'Deleted tags'; /* LOC */

	useLocationStateStore(deletedTagsStore);

	return (
		<Layout title={title} pageTitle={title} ready>
			<DebounceInput
				type="text"
				value={deletedTagsStore.searchTerm}
				onChange={(e): void =>
					runInAction(() => {
						deletedTagsStore.searchTerm = e.target.value;
					})
				}
				maxLength={200}
				debounceTimeout={300}
			/>
			<TagSearchList tagSearchStore={deletedTagsStore} />
		</Layout>
	);
});

export default AdminDeletedTags;
