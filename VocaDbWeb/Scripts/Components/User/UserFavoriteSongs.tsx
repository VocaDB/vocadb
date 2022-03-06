import Breadcrumb from '@Bootstrap/Breadcrumb';
import UserDetailsContract from '@DataContracts/User/UserDetailsContract';
import ArtistRepository from '@Repositories/ArtistRepository';
import SongRepository from '@Repositories/SongRepository';
import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import PVPlayersFactory from '@Stores/PVs/PVPlayersFactory';
import RatedSongsSearchStore from '@Stores/User/RatedSongsSearchStore';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

import '../../../wwwroot/Content/Styles/songlist.less';
import Layout from '../Shared/Layout';
import useScript from '../useScript';
import useStoreWithPaging from '../useStoreWithPaging';
import useVocaDbTitle from '../useVocaDbTitle';
import RatedSongs from './Partials/RatedSongs';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const userRepo = new UserRepository(httpClient, urlMapper);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);
const songRepo = new SongRepository(httpClient, vdb.values.baseAddress);
const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);

const pvPlayersFactory = new PVPlayersFactory();

interface UserFavoriteSongsLayoutProps {
	user: UserDetailsContract;
	ratedSongsStore: RatedSongsSearchStore;
}

const UserFavoriteSongsLayout = ({
	user,
	ratedSongsStore,
}: UserFavoriteSongsLayoutProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	const title = `Songs rated by ${user.name}`; /* TODO: localize */

	useVocaDbTitle(title, true);

	useStoreWithPaging(ratedSongsStore);

	useScript('/Scripts/soundcloud-api.js');
	useScript('https://www.youtube.com/iframe_api');

	return (
		<Layout
			title={title}
			parents={
				<>
					<Breadcrumb.Item linkAs={Link} linkProps={{ to: '/User' }}>
						{t('ViewRes:Shared.Users')}
					</Breadcrumb.Item>
				</>
			}
		>
			<RatedSongs ratedSongsStore={ratedSongsStore} />
		</Layout>
	);
};

const UserFavoriteSongs = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<{
		user: UserDetailsContract;
		ratedSongsStore: RatedSongsSearchStore;
	}>();

	React.useEffect(() => {
		userRepo
			.getOne({ id: Number(id) })
			.then((user) => userRepo.getDetails({ name: user.name! }))
			.then((user) =>
				setModel({
					user: user,
					ratedSongsStore: new RatedSongsSearchStore(
						vdb.values,
						urlMapper,
						userRepo,
						artistRepo,
						songRepo,
						tagRepo,
						user.id,
						pvPlayersFactory,
						true,
					),
				}),
			)
			.catch((error) => {
				if (error.response) {
					if (error.response.status === 404)
						window.location.href = '/Error/NotFound';
				}

				throw error;
			});
	}, [id]);

	return model ? (
		<UserFavoriteSongsLayout
			user={model.user}
			ratedSongsStore={model.ratedSongsStore}
		/>
	) : (
		<></>
	);
};

export default UserFavoriteSongs;
