import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import AlbumSearchList from '@/Pages/Search/Partials/AlbumSearchList';
import { albumRepo } from '@/Repositories/AlbumRepository';
import { DeletedAlbumsStore } from '@/Stores/Album/DeletedAlbumsStore';
import { useVdb } from '@/VdbContext';
import { useLocationStateStore } from '@/route-sphere';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const AlbumDeleted = observer((): React.ReactElement => {
	const vdb = useVdb();

	const [deletedAlbumsStore] = React.useState(
		() => new DeletedAlbumsStore(vdb.values, albumRepo),
	);

	const { t } = useTranslation(['ViewRes']);

	const title = 'Deleted albums'; /* LOC */

	useLocationStateStore(deletedAlbumsStore);

	return (
		<Layout
			pageTitle={title}
			ready={true}
			title={title}
			parents={
				<>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: '/Album',
						}}
					>
						{t('ViewRes:Shared.Albums')}
					</Breadcrumb.Item>
				</>
			}
		>
			<DebounceInput
				type="text"
				value={deletedAlbumsStore.searchTerm}
				onChange={(e): void =>
					runInAction(() => {
						deletedAlbumsStore.searchTerm = e.target.value;
					})
				}
				maxLength={200}
				debounceTimeout={300}
			/>

			<AlbumSearchList albumSearchStore={deletedAlbumsStore} />
		</Layout>
	);
});

export default AlbumDeleted;
