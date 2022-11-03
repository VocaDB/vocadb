import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { useVdbTitle } from '@/Components/useVdbTitle';
import AlbumSearchList from '@/Pages/Search/Partials/AlbumSearchList';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { DeletedAlbumsStore } from '@/Stores/Album/DeletedAlbumsStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const httpClient = new HttpClient();

const albumRepo = new AlbumRepository(httpClient, vdb.values.baseAddress);

const deletedAlbumsStore = new DeletedAlbumsStore(vdb.values, albumRepo);

const AlbumDeleted = observer(
	(): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const title = 'Deleted albums'; /* LOCALIZE */

		useVdbTitle(title, true);

		useLocationStateStore(deletedAlbumsStore);

		return (
			<Layout
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
	},
);

export default AlbumDeleted;
